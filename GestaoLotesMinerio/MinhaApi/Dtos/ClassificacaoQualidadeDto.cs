namespace MinhaApi.Dtos
{
    public class ClassificacaoQualidadeDto
    {
        public int LoteId { get; set; }
        public string CodigoLote { get; set; } = "";
        public string Classificacao { get; set; } = ""; // "Premium", "Padrão", "Baixa"
        public decimal TeorFe { get; set; }
        public decimal Umidade { get; set; }
        public decimal? SiO2 { get; set; }
        public string Observacao { get; set; } = "";
    }
}
