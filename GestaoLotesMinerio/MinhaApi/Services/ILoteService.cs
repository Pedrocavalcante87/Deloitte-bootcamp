using MinhaApi.Dtos;

namespace MinhaApi.Services
{
    public interface ILoteService
    {
        // 1. Classificação de Qualidade (GET)
        ClassificacaoQualidadeDto ClassificarQualidade(int loteId);

        // 2. Preço por Tonelada (GET)
        PrecoToneladaDto CalcularPreco(int loteId);

        // 3. Histórico de Movimentação (POST)
        HistoricoMovimentacaoDto AdicionarHistorico(int loteId, string localDestino);

        // 4. Avançar Status do Lote (POST)
        HistoricoMovimentacaoDto AvancarStatus(int loteId);

        // 5. Penalidade por Umidade (GET)
        PenalidadeUmidadeDto CalcularPenalidadeUmidade(int loteId);
    }
}
