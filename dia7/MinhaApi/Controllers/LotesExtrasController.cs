using Microsoft.AspNetCore.Mvc;
using MinhaApi.Services;

namespace MinhaApi.Controllers
{
    [ApiController]
    [Route("api/lotes")]
    public class LotesExtrasController : ControllerBase
    {
        private readonly ILoteService _loteService;

        public LotesExtrasController(ILoteService loteService)
        {
            _loteService = loteService;
        }

        /// <summary>
        /// 1. Classificação de Qualidade (GET)
        /// Retorna "Premium / Padrão / Baixa" baseado em Fe, Umidade e SiO₂
        /// </summary>
        /// <param name="id">ID do lote</param>
        /// <returns>Classificação de qualidade do lote</returns>
        [HttpGet("{id}/classificacao")]
        public IActionResult ClassificarQualidade(int id)
        {
            try
            {
                var resultado = _loteService.ClassificarQualidade(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 2. Preço por Tonelada (GET)
        /// Usa a classificação para calcular preço e valor total
        /// </summary>
        /// <param name="id">ID do lote</param>
        /// <returns>Preço por tonelada e valor total do lote</returns>
        [HttpGet("{id}/preco")]
        public IActionResult CalcularPreco(int id)
        {
            try
            {
                var resultado = _loteService.CalcularPreco(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 5. Penalidade por Umidade (GET)
        /// Cálculo financeiro baseado no excesso de umidade
        /// </summary>
        /// <param name="id">ID do lote</param>
        /// <returns>Cálculo de penalidade por excesso de umidade</returns>
        [HttpGet("{id}/penalidade-umidade")]
        public IActionResult CalcularPenalidadeUmidade(int id)
        {
            try
            {
                var resultado = _loteService.CalcularPenalidadeUmidade(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 3. Histórico de Movimentação (POST)
        /// Adiciona auditoria de updates: local, status, data
        /// </summary>
        /// <param name="id">ID do lote</param>
        /// <param name="localDestino">Local de destino (query parameter)</param>
        /// <returns>Histórico da movimentação registrada</returns>
        [HttpPost("{id}/movimentacao")]
        public IActionResult AdicionarHistorico(int id, [FromQuery] string localDestino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(localDestino))
                {
                    return BadRequest(new { message = "O parâmetro 'localDestino' é obrigatório." });
                }

                var resultado = _loteService.AdicionarHistorico(id, localDestino);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// 4. Avançar Status do Lote (POST)
        /// Simula o fluxo logístico real: Estoque → Transporte → Embarcado
        /// </summary>
        /// <param name="id">ID do lote</param>
        /// <returns>Histórico do avanço de status</returns>
        [HttpPost("{id}/avancar-status")]
        public IActionResult AvancarStatus(int id)
        {
            try
            {
                var resultado = _loteService.AvancarStatus(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
