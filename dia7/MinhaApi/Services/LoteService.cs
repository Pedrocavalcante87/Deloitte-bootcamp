using MinhaApi.Data;
using MinhaApi.Dtos;
using MinhaApi.Models;

namespace MinhaApi.Services
{
    public class LoteService : ILoteService
    {
        private readonly AppDbContext _db;

        public LoteService(AppDbContext db)
        {
            _db = db;
        }

        // 1. Classificação de Qualidade
        // Lógica:
        // - Premium: TeorFe >= 67%, Umidade <= 8%, SiO2 <= 2%
        // - Padrão: TeorFe >= 63%, Umidade <= 10%, SiO2 <= 4%
        // - Baixa: Demais casos
        public ClassificacaoQualidadeDto ClassificarQualidade(int loteId)
        {
            var lote = _db.LotesMinerio.FirstOrDefault(x => x.Id == loteId);

            if (lote == null)
            {
                throw new Exception($"Lote com ID {loteId} não encontrado.");
            }

            string classificacao;
            string observacao;

            // Verifica critérios Premium
            if (lote.TeorFe >= 67 && lote.Umidade <= 8 && (lote.SiO2 == null || lote.SiO2 <= 2))
            {
                classificacao = "Premium";
                observacao = "Lote de alta qualidade. Atende todos os critérios premium.";
            }
            // Verifica critérios Padrão
            else if (lote.TeorFe >= 63 && lote.Umidade <= 10 && (lote.SiO2 == null || lote.SiO2 <= 4))
            {
                classificacao = "Padrão";
                observacao = "Lote de qualidade padrão. Atende especificações comerciais.";
            }
            // Classificação Baixa
            else
            {
                classificacao = "Baixa";
                observacao = "Lote de qualidade abaixo do padrão. Pode requerer beneficiamento.";
            }

            return new ClassificacaoQualidadeDto
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                Classificacao = classificacao,
                TeorFe = lote.TeorFe,
                Umidade = lote.Umidade,
                SiO2 = lote.SiO2,
                Observacao = observacao
            };
        }

        // 2. Preço por Tonelada
        // Lógica:
        // - Premium: R$ 850/t
        // - Padrão: R$ 650/t
        // - Baixa: R$ 450/t
        public PrecoToneladaDto CalcularPreco(int loteId)
        {
            var classificacao = ClassificarQualidade(loteId);
            var lote = _db.LotesMinerio.FirstOrDefault(x => x.Id == loteId);

            if (lote == null)
            {
                throw new Exception($"Lote com ID {loteId} não encontrado.");
            }

            decimal precoPorTonelada;

            switch (classificacao.Classificacao)
            {
                case "Premium":
                    precoPorTonelada = 850m;
                    break;
                case "Padrão":
                    precoPorTonelada = 650m;
                    break;
                case "Baixa":
                    precoPorTonelada = 450m;
                    break;
                default:
                    precoPorTonelada = 450m;
                    break;
            }

            decimal valorTotal = precoPorTonelada * lote.Toneladas;

            return new PrecoToneladaDto
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                Classificacao = classificacao.Classificacao,
                Toneladas = lote.Toneladas,
                PrecoPorTonelada = precoPorTonelada,
                ValorTotal = valorTotal,
                Moeda = "BRL"
            };
        }

        // 3. Histórico de Movimentação
        // Adiciona auditoria de updates: local, status, data
        public HistoricoMovimentacaoDto AdicionarHistorico(int loteId, string localDestino)
        {
            var lote = _db.LotesMinerio.FirstOrDefault(x => x.Id == loteId);

            if (lote == null)
            {
                throw new Exception($"Lote com ID {loteId} não encontrado.");
            }

            string localOrigem = lote.LocalizacaoAtual;
            string statusAnterior = ObterNomeStatus(lote.Status);

            // Atualiza a localização
            lote.LocalizacaoAtual = localDestino;
            _db.SaveChanges();

            return new HistoricoMovimentacaoDto
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                DataMovimentacao = DateTime.Now,
                LocalOrigem = localOrigem,
                LocalDestino = localDestino,
                StatusAnterior = statusAnterior,
                StatusAtual = ObterNomeStatus(lote.Status),
                Mensagem = $"Lote movimentado de '{localOrigem}' para '{localDestino}'"
            };
        }

        // 4. Avançar Status do Lote
        // Fluxo: EmEstoque (0) → EmTransporte (1) → Embarcado (2)
        public HistoricoMovimentacaoDto AvancarStatus(int loteId)
        {
            var lote = _db.LotesMinerio.FirstOrDefault(x => x.Id == loteId);

            if (lote == null)
            {
                throw new Exception($"Lote com ID {loteId} não encontrado.");
            }

            string statusAnterior = ObterNomeStatus(lote.Status);
            string localOrigem = lote.LocalizacaoAtual;
            string localDestino;
            string mensagem;

            // Avança o status
            switch (lote.Status)
            {
                case StatusLote.EmEstoque:
                    lote.Status = StatusLote.EmTransporte;
                    localDestino = "Em trânsito - Via Férrea";
                    lote.LocalizacaoAtual = localDestino;
                    mensagem = $"Lote avançou de '{statusAnterior}' para 'EmTransporte'";
                    break;

                case StatusLote.EmTransporte:
                    lote.Status = StatusLote.Embarcado;
                    localDestino = "Porto - Navio atracado";
                    lote.LocalizacaoAtual = localDestino;
                    mensagem = $"Lote avançou de '{statusAnterior}' para 'Embarcado'";
                    break;

                case StatusLote.Embarcado:
                    // Já está no status final
                    localDestino = lote.LocalizacaoAtual;
                    mensagem = "Lote já está no status final 'Embarcado'. Não é possível avançar.";
                    break;

                default:
                    throw new Exception("Status desconhecido.");
            }

            _db.SaveChanges();

            return new HistoricoMovimentacaoDto
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                DataMovimentacao = DateTime.Now,
                LocalOrigem = localOrigem,
                LocalDestino = localDestino,
                StatusAnterior = statusAnterior,
                StatusAtual = ObterNomeStatus(lote.Status),
                Mensagem = mensagem
            };
        }

        // 5. Penalidade por Umidade
        // Lógica:
        // - Umidade máxima permitida: 10%
        // - Para cada 1% acima: penalidade de R$ 15/tonelada
        public PenalidadeUmidadeDto CalcularPenalidadeUmidade(int loteId)
        {
            var lote = _db.LotesMinerio.FirstOrDefault(x => x.Id == loteId);

            if (lote == null)
            {
                throw new Exception($"Lote com ID {loteId} não encontrado.");
            }

            decimal umidadeMaxima = 10m;
            decimal penalidadePorPorcentoPorTonelada = 15m; // R$ 15/t por cada 1% de excesso

            decimal excessoUmidade = 0m;
            decimal valorPenalidadePorTonelada = 0m;
            decimal valorTotalPenalidade = 0m;
            bool temPenalidade = false;
            string observacao;

            if (lote.Umidade > umidadeMaxima)
            {
                excessoUmidade = lote.Umidade - umidadeMaxima;
                valorPenalidadePorTonelada = excessoUmidade * penalidadePorPorcentoPorTonelada;
                valorTotalPenalidade = valorPenalidadePorTonelada * lote.Toneladas;
                temPenalidade = true;
                observacao = $"Umidade acima do limite. Penalidade aplicada: {excessoUmidade:F2}% x R$ {penalidadePorPorcentoPorTonelada}/t/% = R$ {valorPenalidadePorTonelada:F2}/t";
            }
            else
            {
                observacao = "Umidade dentro do limite permitido. Sem penalidades.";
            }

            return new PenalidadeUmidadeDto
            {
                LoteId = lote.Id,
                CodigoLote = lote.CodigoLote,
                UmidadeAtual = lote.Umidade,
                UmidadeMaximaPermitida = umidadeMaxima,
                ExcessoUmidade = excessoUmidade,
                Toneladas = lote.Toneladas,
                ValorPenalidadePorTonelada = valorPenalidadePorTonelada,
                ValorTotalPenalidade = valorTotalPenalidade,
                TemPenalidade = temPenalidade,
                Observacao = observacao
            };
        }

        // Método auxiliar para obter o nome do status
        private string ObterNomeStatus(StatusLote status)
        {
            return status switch
            {
                StatusLote.EmEstoque => "EmEstoque",
                StatusLote.EmTransporte => "EmTransporte",
                StatusLote.Embarcado => "Embarcado",
                _ => "Desconhecido"
            };
        }
    }
}
