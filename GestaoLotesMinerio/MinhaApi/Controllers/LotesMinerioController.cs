using Microsoft.AspNetCore.Mvc;
using MinhaApi.Data;
using MinhaApi.Dtos;
using MinhaApi.Models;
using MinhaApi.Services;

namespace MinhaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotesMinerioController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ICacheService _cache;
    private readonly ILoteQueueProducer _queueProducer;

    public LotesMinerioController(AppDbContext db, ICacheService cache, ILoteQueueProducer queueProducer)
    {
        _db = db;
        _cache = cache;
        _queueProducer = queueProducer;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoteMinerioDto input)
    {
        if (input == null)
            return BadRequest(new { message = "Corpo da requisição é obrigatório." });

        // Obrigatórios
        if (string.IsNullOrWhiteSpace(input.CodigoLote))
            return BadRequest(new { message = "CodigoLote é obrigatório." });

        if (string.IsNullOrWhiteSpace(input.MinaOrigem))
            return BadRequest(new { message = "MinaOrigem é obrigatório." });

        if (string.IsNullOrWhiteSpace(input.LocalizacaoAtual))
            return BadRequest(new { message = "LocalizacaoAtual é obrigatório." });

        // Intervalos
        if (input.Toneladas <= 0)
            return BadRequest(new { message = "Toneladas deve ser maior que zero." });

        if (input.TeorFe < 0 || input.TeorFe > 100)
            return BadRequest(new { message = "TeorFe deve estar entre 0 e 100." });

        if (input.Umidade < 0 || input.Umidade > 100)
            return BadRequest(new { message = "Umidade deve estar entre 0 e 100." });

        if (input.SiO2 is not null && (input.SiO2 < 0 || input.SiO2 > 100))
            return BadRequest(new { message = "SiO2 deve estar entre 0 e 100 (ou nulo)." });

        if (input.P is not null && (input.P < 0 || input.P > 999.999m))
            return BadRequest(new { message = "P deve estar entre 0 e 999.999 (ou nulo)." });

        if (!Enum.IsDefined(typeof(StatusLote), input.Status))
            return BadRequest(new { message = "Status inválido (0, 1 ou 2)." });

        // Regra: código único
        bool jaExiste = _db.LotesMinerio.Any(x => x.CodigoLote == input.CodigoLote);
        if (jaExiste)
            return Conflict(new { message = "Já existe um lote com esse CodigoLote." });

        var entity = new LoteMinerio
        {
            CodigoLote = input.CodigoLote.Trim(),
            MinaOrigem = input.MinaOrigem.Trim(),
            TeorFe = input.TeorFe,
            Umidade = input.Umidade,
            SiO2 = input.SiO2,
            P = input.P,
            Toneladas = input.Toneladas,
            DataProducao = input.DataProducao ?? DateTime.Now,
            Status = (StatusLote)input.Status,
            LocalizacaoAtual = input.LocalizacaoAtual.Trim()
        };

        _db.LotesMinerio.Add(entity);
        _db.SaveChanges();

        // Salva no cache também
        var cacheKey = $"lote:{entity.Id}";
        await _cache.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(5));

        // Enfileira para processamento assíncrono
        var mensagem = new ProcessarLoteMessage
        {
            LoteId = entity.Id,
            CodigoLote = entity.CodigoLote,
            TeorFe = entity.TeorFe,
            Umidade = entity.Umidade,
            Toneladas = entity.Toneladas,
            MinaOrigem = entity.MinaOrigem,
            Acao = "RecalcularClassificacao"
        };

        await _queueProducer.EnfileirarAsync(mensagem);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        // 1. Tenta buscar do cache
        var cacheKey = $"lote:{id}";
        var cachedLote = await _cache.GetAsync<LoteMinerio>(cacheKey);

        if (cachedLote != null)
        {
            return Ok(new { source = "cache", data = cachedLote });
        }

        // 2. Se não existe no cache, busca do banco
        var entity = _db.LotesMinerio.FirstOrDefault(x => x.Id == id);
        if (entity == null)
            return NotFound(new { message = "Lote não encontrado." });

        // 3. Salva no cache por 5 minutos
        await _cache.SetAsync(cacheKey, entity, TimeSpan.FromMinutes(5));

        return Ok(new { source = "database", data = entity });
    }

    [HttpGet]
    public IActionResult GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? status = null,
        [FromQuery] string? codigo = null,
        [FromQuery] string? mina = null
    )
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var query = _db.LotesMinerio.AsQueryable();

        if (status is not null)
        {
            if (!Enum.IsDefined(typeof(StatusLote), status.Value))
                return BadRequest(new { message = "Status inválido (0, 1 ou 2)." });

            query = query.Where(x => (int)x.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(codigo))
            query = query.Where(x => x.CodigoLote.Contains(codigo.Trim()));

        if (!string.IsNullOrWhiteSpace(mina))
            query = query.Where(x => x.MinaOrigem.Contains(mina.Trim()));

        int total = query.Count();

        var itens = query
            .OrderByDescending(x => x.DataProducao)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new { total, page, pageSize, itens });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLoteMinerioDto input)
    {
        if (input == null)
            return BadRequest(new { message = "Corpo da requisição é obrigatório." });

        var entity = _db.LotesMinerio.FirstOrDefault(x => x.Id == id);
        if (entity == null)
            return NotFound(new { message = "Lote não encontrado." });

        // Obrigatórios
        if (string.IsNullOrWhiteSpace(input.CodigoLote))
            return BadRequest(new { message = "CodigoLote é obrigatório." });

        if (string.IsNullOrWhiteSpace(input.MinaOrigem))
            return BadRequest(new { message = "MinaOrigem é obrigatório." });

        if (string.IsNullOrWhiteSpace(input.LocalizacaoAtual))
            return BadRequest(new { message = "LocalizacaoAtual é obrigatório." });

        // Intervalos
        if (input.Toneladas <= 0)
            return BadRequest(new { message = "Toneladas deve ser maior que zero." });

        if (input.TeorFe < 0 || input.TeorFe > 100)
            return BadRequest(new { message = "TeorFe deve estar entre 0 e 100." });

        if (input.Umidade < 0 || input.Umidade > 100)
            return BadRequest(new { message = "Umidade deve estar entre 0 e 100." });

        if (input.SiO2 is not null && (input.SiO2 < 0 || input.SiO2 > 100))
            return BadRequest(new { message = "SiO2 deve estar entre 0 e 100 (ou nulo)." });

        if (input.P is not null && (input.P < 0 || input.P > 999.999m))
            return BadRequest(new { message = "P deve estar entre 0 e 999.999 (ou nulo)." });

        if (!Enum.IsDefined(typeof(StatusLote), input.Status))
            return BadRequest(new { message = "Status inválido (0, 1 ou 2)." });

        // Regra: código único (se mudou)
        bool codigoEmUso = _db.LotesMinerio.Any(x => x.CodigoLote == input.CodigoLote && x.Id != id);
        if (codigoEmUso)
            return Conflict(new { message = "Já existe outro lote com esse CodigoLote." });

        // Atualiza
        entity.CodigoLote = input.CodigoLote.Trim();
        entity.MinaOrigem = input.MinaOrigem.Trim();
        entity.TeorFe = input.TeorFe;
        entity.Umidade = input.Umidade;
        entity.SiO2 = input.SiO2;
        entity.P = input.P;
        entity.Toneladas = input.Toneladas;
        entity.DataProducao = input.DataProducao ?? entity.DataProducao;
        entity.Status = (StatusLote)input.Status;
        entity.LocalizacaoAtual = input.LocalizacaoAtual.Trim();

        _db.SaveChanges();

        // Invalida o cache
        var cacheKey = $"lote:{id}";
        await _cache.RemoveAsync(cacheKey);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = _db.LotesMinerio.FirstOrDefault(x => x.Id == id);
        if (entity == null)
            return NotFound(new { message = "Lote não encontrado." });

        _db.LotesMinerio.Remove(entity);
        _db.SaveChanges();

        // Invalida o cache
        var cacheKey = $"lote:{id}";
        await _cache.RemoveAsync(cacheKey);

        return NoContent();
    }
}
