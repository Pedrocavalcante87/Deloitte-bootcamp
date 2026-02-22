namespace MinhaApi.Dtos
{
    public class PenalidadeUmidadeDto
    {
        public int LoteId { get; set; }
        public string CodigoLote { get; set; } = "";
        public decimal UmidadeAtual { get; set; }
        public decimal UmidadeMaximaPermitida { get; set; }
        public decimal ExcessoUmidade { get; set; }
        public decimal Toneladas { get; set; }
        public decimal ValorPenalidadePorTonelada { get; set; }
        public decimal ValorTotalPenalidade { get; set; }
        public bool TemPenalidade { get; set; }
        public string Observacao { get; set; } = "";
    }
}
