using System.Text.Json;
using MinhaApi.Dtos;
using StackExchange.Redis;

namespace MinhaApi.Services;

/// <summary>
/// Implementação do Producer que envia mensagens para o Redis Stream
/// </summary>
public class LoteQueueProducer : ILoteQueueProducer
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<LoteQueueProducer> _logger;
    private const string StreamKey = "lotes-stream";

    public LoteQueueProducer(IConnectionMultiplexer redis, ILogger<LoteQueueProducer> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<string> EnfileirarAsync(ProcessarLoteMessage mensagem)
    {
        try
        {
            var db = _redis.GetDatabase();

            // Serializa a mensagem em JSON
            var payload = JsonSerializer.Serialize(mensagem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Adiciona no stream usando XADD
            var entries = new[]
            {
                new NameValueEntry("type", mensagem.Acao),
                new NameValueEntry("payload", payload)
            };

            var messageId = await db.StreamAddAsync(StreamKey, entries);

            _logger.LogInformation(
                "[Producer] Lote {CodigoLote} (Id={LoteId}) enfileirado no Redis Stream com MessageId={MessageId}",
                mensagem.CodigoLote,
                mensagem.LoteId,
                messageId
            );

            return messageId.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Producer] Erro ao enfileirar lote {CodigoLote}", mensagem.CodigoLote);
            throw;
        }
    }
}
