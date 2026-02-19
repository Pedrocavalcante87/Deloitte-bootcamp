namespace MinhaApi.Dtos
{
    public class HistoricoMovimentacaoDto
    {
        public int LoteId { get; set; }
        public string CodigoLote { get; set; } = "";
        public DateTime DataMovimentacao { get; set; }
        public string LocalOrigem { get; set; } = "";
        public string LocalDestino { get; set; } = "";
        public string StatusAnterior { get; set; } = "";
        public string StatusAtual { get; set; } = "";
        public string Mensagem { get; set; } = "";
    }
}
