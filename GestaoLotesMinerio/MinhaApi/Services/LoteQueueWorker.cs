using System.Text.Json;
using MinhaApi.Dtos;
using StackExchange.Redis;

namespace MinhaApi.Services;

/// <summary>
/// Worker (Consumer) que processa mensagens do Redis Stream em background
/// </summary>
public class LoteQueueWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<LoteQueueWorker> _logger;

    private const string StreamKey = "lotes-stream";
    private const string ConsumerGroup = "grupo-lotes";
    private const string ConsumerName = "worker-1";
    private const string DlqStreamKey = "lotes-dlq";
    private const int MaxDeliveries = 3;
    private const int MinIdleTimeMs = 10000; // 10 segundos

    public LoteQueueWorker(IConnectionMultiplexer redis, ILogger<LoteQueueWorker> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Worker] Iniciando LoteQueueWorker...");

        var db = _redis.GetDatabase();

        // Garante que o Consumer Group existe
        await GarantirConsumerGroupAsync(db);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Reprocessa mensagens pendentes (mensagens antigas que não foram ACKadas)
                await ReprocessarPendentesAsync(db, stoppingToken);

                // 2. Lê novas mensagens do stream
                var streamEntries = await db.StreamReadGroupAsync(
                    StreamKey,
                    ConsumerGroup,
                    ConsumerName,
                    ">", // ">" significa: apenas mensagens novas
                    count: 10 // Processa até 10 mensagens por vez
                );

                if (streamEntries.Length == 0)
                {
                    // Sem mensagens novas, aguarda um pouco antes de checar novamente
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                // 3. Processa cada mensagem
                foreach (var entry in streamEntries)
                {
                    await ProcessarMensagemAsync(db, entry, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("[Worker] Worker cancelado.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Worker] Erro no loop principal do Worker.");
                await Task.Delay(5000, stoppingToken); // Aguarda 5s antes de tentar novamente
            }
        }

        _logger.LogInformation("[Worker] LoteQueueWorker finalizado.");
    }

    private async Task GarantirConsumerGroupAsync(IDatabase db)
    {
        try
        {
            // Tenta criar o consumer group (XGROUP CREATE)
            await db.StreamCreateConsumerGroupAsync(StreamKey, ConsumerGroup, StreamPosition.NewMessages);
            _logger.LogInformation("[Worker] Consumer Group '{ConsumerGroup}' criado com sucesso.", ConsumerGroup);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            // Consumer group já existe, tudo certo
            _logger.LogDebug("[Worker] Consumer Group '{ConsumerGroup}' já existe.", ConsumerGroup);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Worker] Erro ao criar Consumer Group '{ConsumerGroup}'.", ConsumerGroup);
            throw;
        }
    }

    private async Task ReprocessarPendentesAsync(IDatabase db, CancellationToken stoppingToken)
    {
        try
        {
            // XAUTOCLAIM: reclama mensagens pendentes com idle time > 10s
            var claimedEntries = await db.StreamAutoClaimAsync(
                StreamKey,
                ConsumerGroup,
                ConsumerName,
                MinIdleTimeMs,
                StreamPosition.Beginning,
                count: 10
            );

            if (claimedEntries.ClaimedEntries.Length > 0)
            {
                _logger.LogInformation("[Worker] {Count} mensagens pendentes reclamadas para reprocessamento.",
                    claimedEntries.ClaimedEntries.Length);

                foreach (var entry in claimedEntries.ClaimedEntries)
                {
                    await ProcessarMensagemAsync(db, entry, stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Worker] Erro ao reprocessar mensagens pendentes.");
        }
    }

    private async Task ProcessarMensagemAsync(IDatabase db, StreamEntry entry, CancellationToken stoppingToken)
    {
        var messageId = entry.Id;

        try
        {
            // Extrai o payload da mensagem
            var payloadField = entry.Values.FirstOrDefault(x => x.Name == "payload");
            if (payloadField.Name.IsNullOrEmpty || payloadField.Value.IsNullOrEmpty)
            {
                _logger.LogWarning("[Worker] Mensagem {MessageId} sem campo 'payload'. Fazendo ACK...", messageId);
                await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, messageId);
                return;
            }

            var payload = payloadField.Value.ToString();
            var mensagem = JsonSerializer.Deserialize<ProcessarLoteMessage>(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (mensagem == null)
            {
                _logger.LogWarning("[Worker] Mensagem {MessageId} com payload inválido. Fazendo ACK...", messageId);
                await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, messageId);
                return;
            }

            // Verifica quantas vezes a mensagem já foi entregue
            var deliveryCount = await ObterDeliveryCountAsync(db, messageId);

            if (deliveryCount >= MaxDeliveries)
            {
                // Move para DLQ (Dead Letter Queue)
                _logger.LogWarning(
                    "[Worker] Mensagem {MessageId} excedeu {MaxDeliveries} tentativas. Movendo para DLQ...",
                    messageId,
                    MaxDeliveries
                );
                await MoverParaDlqAsync(db, entry, mensagem);
                await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, messageId);
                return;
            }

            // Processa a mensagem
            await ProcessarAsync(mensagem, stoppingToken);

            // ACK (marca como processada)
            await db.StreamAcknowledgeAsync(StreamKey, ConsumerGroup, messageId);

            _logger.LogInformation(
                "[Worker] Mensagem {MessageId} processada e ACKada com sucesso.",
                messageId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Worker] Erro ao processar mensagem {MessageId}. Será reprocessada.", messageId);
            // Não faz ACK, a mensagem ficará pendente para reprocessamento
        }
    }

    private async Task<int> ObterDeliveryCountAsync(IDatabase db, RedisValue messageId)
    {
        try
        {
            // XPENDING retorna informações sobre mensagens pendentes
            var pending = await db.StreamPendingMessagesAsync(
                StreamKey,
                ConsumerGroup,
                1, // count
                ConsumerName
            );

            if (pending.Length > 0)
            {
                // Procura a mensagem específica
                var msg = pending.FirstOrDefault(p => p.MessageId == messageId);
                if (msg.MessageId != RedisValue.Null)
                {
                    return (int)msg.DeliveryCount;
                }
            }

            return 0;
        }
        catch
        {
            return 0;
        }
    }

    private async Task MoverParaDlqAsync(IDatabase db, StreamEntry entry, ProcessarLoteMessage mensagem)
    {
        try
        {
            var dlqPayload = JsonSerializer.Serialize(new
            {
                originalMessageId = entry.Id.ToString(),
                mensagem,
                dataMovimentacao = DateTime.Now,
                motivo = "Excedeu número máximo de tentativas"
            });

            await db.StreamAddAsync(DlqStreamKey, new[]
            {
                new NameValueEntry("type", "dlq"),
                new NameValueEntry("payload", dlqPayload)
            });

            _logger.LogWarning("[Worker] Mensagem {MessageId} movida para DLQ.", entry.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Worker] Erro ao mover mensagem {MessageId} para DLQ.", entry.Id);
        }
    }

    private async Task ProcessarAsync(ProcessarLoteMessage mensagem, CancellationToken stoppingToken)
    {
        // Simula processamento (classificação do lote)
        _logger.LogInformation(
            "[Fila] Processando lote {CodigoLote} (Id={LoteId}) | Ação={Acao}",
            mensagem.CodigoLote,
            mensagem.LoteId,
            mensagem.Acao
        );

        // Lógica de classificação baseada em TeorFe e Umidade
        string classificacao;

        if (mensagem.TeorFe >= 65 && mensagem.Umidade <= 8)
            classificacao = "Premium";
        else if (mensagem.TeorFe >= 60 && mensagem.Umidade <= 10)
            classificacao = "Padrão";
        else
            classificacao = "Baixa Qualidade";

        // Simula I/O (ex: salvar em banco, chamar serviço externo, etc)
        await Task.Delay(200, stoppingToken);

        _logger.LogInformation(
            "[Fila] Lote {CodigoLote} (Id={LoteId}) -> Ação={Acao} | Classificação={Classificacao}",
            mensagem.CodigoLote,
            mensagem.LoteId,
            mensagem.Acao,
            classificacao
        );
    }
}
