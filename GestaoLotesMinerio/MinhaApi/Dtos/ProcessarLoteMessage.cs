namespace MinhaApi.Dtos;

/// <summary>
/// Mensagem enviada para a fila de processamento de lotes
/// </summary>
public class ProcessarLoteMessage
{
    public int LoteId { get; set; }
    public string CodigoLote { get; set; } = string.Empty;
    public decimal TeorFe { get; set; }
    public decimal Umidade { get; set; }
    public decimal Toneladas { get; set; }
    public string MinaOrigem { get; set; } = string.Empty;
    public string Acao { get; set; } = "RecalcularClassificacao";
    public DateTime DataEnfileiramento { get; set; } = DateTime.Now;
}
