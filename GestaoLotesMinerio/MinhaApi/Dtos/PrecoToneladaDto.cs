namespace MinhaApi.Dtos
{
    public class PrecoToneladaDto
    {
        public int LoteId { get; set; }
        public string CodigoLote { get; set; } = "";
        public string Classificacao { get; set; } = "";
        public decimal Toneladas { get; set; }
        public decimal PrecoPorTonelada { get; set; }
        public decimal ValorTotal { get; set; }
        public string Moeda { get; set; } = "BRL";
    }
}
