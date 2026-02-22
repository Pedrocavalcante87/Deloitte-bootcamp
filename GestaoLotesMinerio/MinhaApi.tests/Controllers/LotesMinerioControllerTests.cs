using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaApi.Controllers;
using MinhaApi.Data;
using MinhaApi.Dtos;
using MinhaApi.Models;
using Xunit;

namespace MinhaApi.Tests.Controllers
{
    public class LotesMinerioControllerTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"db_tests_{Guid.NewGuid()}")
                .Options;

            return new AppDbContext(options);
        }

        private LotesMinerioController CreateController(AppDbContext ctx)
            => new LotesMinerioController(ctx);

        private static LoteMinerio NovoLote(
            string codigo = "L-001",
            string mina = "Carajás",
            decimal teorFe = 65m,
            decimal umidade = 7m,
            decimal toneladas = 1000m,
            StatusLote status = StatusLote.EmEstoque,
            string local = "Pátio Carajás",
            DateTime? data = null,
            decimal? sio2 = null,
            decimal? p = null
        )
            => new()
            {
                CodigoLote = codigo,
                MinaOrigem = mina,
                TeorFe = teorFe,
                Umidade = umidade,
                Toneladas = toneladas,
                Status = status,
                LocalizacaoAtual = local,
                DataProducao = data ?? new DateTime(2026, 2, 9, 10, 0, 0, DateTimeKind.Utc),
                SiO2 = sio2,
                P = p
            };

        // ------------------------------
        // POST /api/LotesMinerio (Create)
        // ------------------------------

        [Fact]
        public void Create_ComDadosValidos_DeveRetornarCreatedEPersistir()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var input = new CreateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-000123",
                MinaOrigem = "Carajás N4E",
                TeorFe = 66.5m,
                Umidade = 8.2m,
                SiO2 = 3.9m,
                P = 0.03m,
                Toneladas = 15000.750m,
                DataProducao = null, // deve assumir DateTime.Now no controller
                Status = 1,          // EmTransporte
                LocalizacaoAtual = "EFVM - Trem 123"
            };

            var before = DateTime.Now;
            var result = controller.Create(input);
            var after = DateTime.Now;

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(LotesMinerioController.GetById), created.ActionName);

            var entity = Assert.IsType<LoteMinerio>(created.Value);
            Assert.True(entity.Id > 0);
            Assert.Equal("MNA-2026-000123", entity.CodigoLote);
            Assert.Equal("Carajás N4E", entity.MinaOrigem);
            Assert.Equal(66.5m, entity.TeorFe);
            Assert.Equal(8.2m, entity.Umidade);
            Assert.Equal(3.9m, entity.SiO2);
            Assert.Equal(0.03m, entity.P);
            Assert.Equal(15000.750m, entity.Toneladas);
            Assert.Equal(StatusLote.EmTransporte, entity.Status);
            Assert.Equal("EFVM - Trem 123", entity.LocalizacaoAtual);

            // DataProducao veio null -> controller usa Now; toleramos janela.
            Assert.True(entity.DataProducao >= before && entity.DataProducao <= after.AddSeconds(5));

            // Confirma persistência no "banco"
            var persisted = ctx.LotesMinerio.AsNoTracking().FirstOrDefault(x => x.Id == entity.Id);
            Assert.NotNull(persisted);
        }

        [Theory]
        [InlineData(null, "Carajás", 10, 10, 100, 1, "Pátio", "CodigoLote é obrigatório.")]
        [InlineData("   ", "Carajás", 10, 10, 100, 1, "Pátio", "CodigoLote é obrigatório.")]
        [InlineData("COD", null, 10, 10, 100, 1, "Pátio", "MinaOrigem é obrigatório.")]
        [InlineData("COD", "   ", 10, 10, 100, 1, "Pátio", "MinaOrigem é obrigatório.")]
        [InlineData("COD", "Mina", 10, 10, 100, 1, null, "LocalizacaoAtual é obrigatório.")]
        [InlineData("COD", "Mina", 10, 10, 100, 1, "   ", "LocalizacaoAtual é obrigatório.")]
        [InlineData("COD", "Mina", -0.1, 10, 100, 1, "Loc", "TeorFe deve estar entre 0 e 100.")]
        [InlineData("COD", "Mina", 100.1, 10, 100, 1, "Loc", "TeorFe deve estar entre 0 e 100.")]
        [InlineData("COD", "Mina", 10, -0.1, 100, 1, "Loc", "Umidade deve estar entre 0 e 100.")]
        [InlineData("COD", "Mina", 10, 100.1, 100, 1, "Loc", "Umidade deve estar entre 0 e 100.")]
        [InlineData("COD", "Mina", 10, 10, 0, 1, "Loc", "Toneladas deve ser maior que zero.")]
        [InlineData("COD", "Mina", 10, 10, 100, -1, "Loc", "Status inválido (0, 1 ou 2).")]
        [InlineData("COD", "Mina", 10, 10, 100, 3, "Loc", "Status inválido (0, 1 ou 2).")]
        public void Create_ComDadosInvalidos_DeveRetornarBadRequest(
            string? codigo, string? mina, decimal teorFe, decimal umidade,
            decimal toneladas, int status, string? local, string mensagemEsperada)
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var input = new CreateLoteMinerioDto
            {
                CodigoLote = codigo ?? "",
                MinaOrigem = mina ?? "",
                TeorFe = teorFe,
                Umidade = umidade,
                Toneladas = toneladas,
                Status = status,
                LocalizacaoAtual = local ?? ""
            };

            var result = controller.Create(input);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var obj = bad.Value;

            // Extrai a mensagem do objeto anônimo { message = "..." }
            var msgProperty = obj?.GetType().GetProperty("message");
            Assert.NotNull(msgProperty);
            var msg = msgProperty!.GetValue(obj) as string;

            Assert.Equal(mensagemEsperada, msg);
        }

        [Fact]
        public void Create_ComCodigoDuplicado_DeveRetornarConflict()
        {
            using var ctx = CreateInMemoryContext();
            // Seed com um lote existente
            ctx.LotesMinerio.Add(NovoLote(codigo: "DUPL-123"));
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var input = new CreateLoteMinerioDto
            {
                CodigoLote = "DUPL-123",
                MinaOrigem = "Carajás",
                TeorFe = 60m,
                Umidade = 5m,
                Toneladas = 100m,
                Status = 0,
                LocalizacaoAtual = "Pátio"
            };

            var result = controller.Create(input);
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            var obj = conflict.Value;

            var msgProperty = obj?.GetType().GetProperty("message");
            Assert.NotNull(msgProperty);
            var msg = msgProperty!.GetValue(obj) as string;

            Assert.Contains("Já existe", msg);
        }

        // ------------------------------
        // GET /api/LotesMinerio/{id}
        // ------------------------------

        [Fact]
        public void GetById_QuandoNaoExiste_DeveRetornarNotFound()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = controller.GetById(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void GetById_QuandoExiste_DeveRetornarOkComEntity()
        {
            using var ctx = CreateInMemoryContext();
            var existente = NovoLote(codigo: "OK-001", status: StatusLote.EmTransporte);
            ctx.LotesMinerio.Add(existente);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = controller.GetById(existente.Id);
            var ok = Assert.IsType<OkObjectResult>(result);

            var entity = Assert.IsType<LoteMinerio>(ok.Value);
            Assert.Equal(existente.Id, entity.Id);
            Assert.Equal(existente.CodigoLote, entity.CodigoLote);
            Assert.Equal(existente.MinaOrigem, entity.MinaOrigem);
            Assert.Equal(existente.TeorFe, entity.TeorFe);
            Assert.Equal(existente.Umidade, entity.Umidade);
            Assert.Equal(existente.SiO2, entity.SiO2);
            Assert.Equal(existente.P, entity.P);
            Assert.Equal(existente.Toneladas, entity.Toneladas);
            Assert.Equal(existente.DataProducao, entity.DataProducao);
            Assert.Equal(existente.Status, entity.Status);
            Assert.Equal(existente.LocalizacaoAtual, entity.LocalizacaoAtual);
        }

        // ------------------------------
        // GET /api/LotesMinerio (GetList)
        // ------------------------------

        [Fact]
        public void GetList_DeveRetornarOkComLista()
        {
            using var ctx = CreateInMemoryContext();
            ctx.LotesMinerio.Add(NovoLote(codigo: "L-1"));
            ctx.LotesMinerio.Add(NovoLote(codigo: "L-2"));
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = controller.GetList();
            var ok = Assert.IsType<OkObjectResult>(result);

            // O controller retorna objeto anônimo { total, page, pageSize, itens }
            var obj = ok.Value;
            Assert.NotNull(obj);

            var itensProperty = obj.GetType().GetProperty("itens");
            Assert.NotNull(itensProperty);
            var itens = itensProperty!.GetValue(obj) as List<LoteMinerio>;

            Assert.NotNull(itens);
            Assert.Equal(2, itens!.Count);
            Assert.Contains(itens, x => x.CodigoLote == "L-1");
            Assert.Contains(itens, x => x.CodigoLote == "L-2");
        }

        [Fact]
        public void GetList_ComFiltroStatus_DeveRetornarApenasDoStatus()
        {
            using var ctx = CreateInMemoryContext();
            ctx.LotesMinerio.Add(NovoLote(codigo: "L-1", status: StatusLote.EmEstoque));
            ctx.LotesMinerio.Add(NovoLote(codigo: "L-2", status: StatusLote.EmTransporte));
            ctx.LotesMinerio.Add(NovoLote(codigo: "L-3", status: StatusLote.EmTransporte));
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = controller.GetList(status: 1); // EmTransporte
            var ok = Assert.IsType<OkObjectResult>(result);

            var obj = ok.Value;
            var itensProperty = obj?.GetType().GetProperty("itens");
            var itens = itensProperty!.GetValue(obj) as List<LoteMinerio>;

            Assert.NotNull(itens);
            Assert.Equal(2, itens!.Count);
            Assert.All(itens, x => Assert.Equal(StatusLote.EmTransporte, x.Status));
        }

        // ------------------------------
        // PUT /api/LotesMinerio/{id}
        // ------------------------------

        [Theory]
        [InlineData(null, "Loc", 10, 10, 100, 1, "MinaOrigem é obrigatório.")]
        [InlineData("   ", "Loc", 10, 10, 100, 1, "MinaOrigem é obrigatório.")]
        [InlineData("Mina", null, 10, 10, 100, 1, "LocalizacaoAtual é obrigatório.")]
        [InlineData("Mina", "   ", 10, 10, 100, 1, "LocalizacaoAtual é obrigatório.")]
        [InlineData("Mina", "Loc", -0.1, 10, 100, 1, "TeorFe deve estar entre 0 e 100.")]
        [InlineData("Mina", "Loc", 100.1, 10, 100, 1, "TeorFe deve estar entre 0 e 100.")]
        [InlineData("Mina", "Loc", 10, -0.1, 100, 1, "Umidade deve estar entre 0 e 100.")]
        [InlineData("Mina", "Loc", 10, 100.1, 100, 1, "Umidade deve estar entre 0 e 100.")]
        [InlineData("Mina", "Loc", 10, 10, 0, 1, "Toneladas deve ser maior que zero.")]
        [InlineData("Mina", "Loc", 10, 10, 100, -1, "Status inválido (0, 1 ou 2).")]
        [InlineData("Mina", "Loc", 10, 10, 100, 3, "Status inválido (0, 1 ou 2).")]
        public void Update_ComDadosInvalidos_DeveRetornarBadRequest(
            string? mina, string? loc, decimal teorFe, decimal umidade, decimal toneladas, int status, string mensagemEsperada)
        {
            using var ctx = CreateInMemoryContext();
            // Adiciona lote existente para testar Update
            var existente = NovoLote(codigo: "TEST-UP");
            ctx.LotesMinerio.Add(existente);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var input = new UpdateLoteMinerioDto
            {
                CodigoLote = "TEST-UP",
                MinaOrigem = mina ?? "",
                LocalizacaoAtual = loc ?? "",
                TeorFe = teorFe,
                Umidade = umidade,
                Toneladas = toneladas,
                Status = status
            };

            var result = controller.Update(existente.Id, input);
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var obj = bad.Value;

            var msgProperty = obj?.GetType().GetProperty("message");
            Assert.NotNull(msgProperty);
            var msg = msgProperty!.GetValue(obj) as string;

            Assert.Equal(mensagemEsperada, msg);
        }

        [Fact]
        public void Update_QuandoNaoExiste_DeveRetornarNotFound()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var input = new UpdateLoteMinerioDto
            {
                CodigoLote = "COD",
                MinaOrigem = "Mina",
                LocalizacaoAtual = "Loc",
                TeorFe = 10,
                Umidade = 10,
                Toneladas = 1,
                Status = 0
            };

            var result = controller.Update(999, input);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Update_ComDadosValidos_DeveRetornarNoContentEAtualizar()
        {
            using var ctx = CreateInMemoryContext();
            var existente = NovoLote(codigo: "UP-001", mina: "Antiga Mina", status: StatusLote.EmEstoque, local: "Antiga Loc");
            ctx.LotesMinerio.Add(existente);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var input = new UpdateLoteMinerioDto
            {
                CodigoLote = "UP-001", // mantém o mesmo código
                MinaOrigem = "Nova Mina",
                TeorFe = 70.5m,
                Umidade = 9.9m,
                SiO2 = 4.2m,
                P = 0.04m,
                Toneladas = 2000.5m,
                DataProducao = existente.DataProducao, // não muda de fato
                Status = 2, // Embarcado
                LocalizacaoAtual = "Nova Loc"
            };

            var result = controller.Update(existente.Id, input);
            Assert.IsType<NoContentResult>(result);

            var atualizado = ctx.LotesMinerio.AsNoTracking().Single(x => x.Id == existente.Id);
            // CodigoLote pode ser atualizado no seu controller, mas mantivemos o mesmo
            Assert.Equal("UP-001", atualizado.CodigoLote);
            Assert.Equal("Nova Mina", atualizado.MinaOrigem);
            Assert.Equal(70.5m, atualizado.TeorFe);
            Assert.Equal(9.9m, atualizado.Umidade);
            Assert.Equal(4.2m, atualizado.SiO2);
            Assert.Equal(0.04m, atualizado.P);
            Assert.Equal(2000.5m, atualizado.Toneladas);
            Assert.Equal(StatusLote.Embarcado, atualizado.Status);
            Assert.Equal("Nova Loc", atualizado.LocalizacaoAtual);
        }

        // ------------------------------
        // DELETE /api/LotesMinerio/{id}
        // ------------------------------

        [Fact]
        public void Delete_QuandoNaoExiste_DeveRetornarNotFound()
        {
            using var ctx = CreateInMemoryContext();
            var controller = CreateController(ctx);

            var result = controller.Delete(12345);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Delete_QuandoExiste_DeveRetornarNoContentERemover()
        {
            using var ctx = CreateInMemoryContext();
            var existente = NovoLote(codigo: "DEL-001");
            ctx.LotesMinerio.Add(existente);
            ctx.SaveChanges();

            var controller = CreateController(ctx);

            var result = controller.Delete(existente.Id);
            Assert.IsType<NoContentResult>(result);

            var aindaExiste = ctx.LotesMinerio.Any(x => x.Id == existente.Id);
            Assert.False(aindaExiste);
        }
    }
}
