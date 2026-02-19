using System;
using MinhaApi.Models;
using Xunit;

namespace MinhaApi.Tests.Models
{
    public class LoteMinerioTests
    {
        [Fact]
        public void Construtor_Padrao_DeveInicializarStringsComoVazias()
        {
            // Arrange & Act
            var lote = new LoteMinerio();

            // Assert
            Assert.NotNull(lote.CodigoLote);
            Assert.Equal(string.Empty, lote.CodigoLote);

            Assert.NotNull(lote.MinaOrigem);
            Assert.Equal(string.Empty, lote.MinaOrigem);

            Assert.NotNull(lote.LocalizacaoAtual);
            Assert.Equal(string.Empty, lote.LocalizacaoAtual);
        }

        [Fact]
        public void Construtor_Padrao_DeveInicializarTiposValorComDefaults()
        {
            // Arrange & Act
            var lote = new LoteMinerio();

            // Assert
            Assert.Equal(0, lote.Id);
            Assert.Equal(0m, lote.TeorFe);
            Assert.Equal(0m, lote.Umidade);
            Assert.Equal(0m, lote.Toneladas);

            // DateTime default é 01/01/0001 00:00:00
            Assert.Equal(default(DateTime), lote.DataProducao);

            // Enum default deve ser 0 => EmEstoque
            Assert.Equal(StatusLote.EmEstoque, lote.Status);

            // Nullable devem iniciar como null
            Assert.Null(lote.SiO2);
            Assert.Null(lote.P);
        }

        [Fact]
        public void Set_Get_DeveAtribuirERecuperarValoresCorretamente()
        {
            // Arrange
            var dataProducao = new DateTime(2026, 2, 9, 10, 30, 0, DateTimeKind.Utc);
            var lote = new LoteMinerio
            {
                Id = 42,
                CodigoLote = "MNA-2026-000123",
                MinaOrigem = "Carajás N4E",
                TeorFe = 66.5m,
                Umidade = 8.25m,
                SiO2 = 4.1m,
                P = 0.03m,
                Toneladas = 15000.75m,
                DataProducao = dataProducao,
                Status = StatusLote.EmTransporte,
                LocalizacaoAtual = "EFVM - Trem 123"
            };

            // Act & Assert
            Assert.Equal(42, lote.Id);
            Assert.Equal("MNA-2026-000123", lote.CodigoLote);
            Assert.Equal("Carajás N4E", lote.MinaOrigem);
            Assert.Equal(66.5m, lote.TeorFe);
            Assert.Equal(8.25m, lote.Umidade);
            Assert.Equal(4.1m, lote.SiO2);
            Assert.Equal(0.03m, lote.P);
            Assert.Equal(15000.75m, lote.Toneladas);
            Assert.Equal(dataProducao, lote.DataProducao);
            Assert.Equal(StatusLote.EmTransporte, lote.Status);
            Assert.Equal("EFVM - Trem 123", lote.LocalizacaoAtual);
        }

        [Fact]
        public void StatusLote_EmEstoque_DeveSerZero()
        {
            // Assert
            Assert.Equal(0, (int)StatusLote.EmEstoque);
        }

        [Fact]
        public void StatusLote_EmTransporte_DeveSerUm()
        {
            // Assert
            Assert.Equal(1, (int)StatusLote.EmTransporte);
        }

        [Fact]
        public void StatusLote_Embarcado_DeveSerDois()
        {
            // Assert
            Assert.Equal(2, (int)StatusLote.Embarcado);
        }

        [Fact]
        public void Status_PodeSerConvertidoDeIntParaEnum()
        {
            // Arrange
            var lote = new LoteMinerio();

            // Act
            lote.Status = (StatusLote)1;

            // Assert
            Assert.Equal(StatusLote.EmTransporte, lote.Status);
        }

        [Fact]
        public void Status_PodeSerConvertidoDeEnumParaInt()
        {
            // Arrange
            var lote = new LoteMinerio
            {
                Status = StatusLote.Embarcado
            };

            // Act & Assert
            Assert.Equal(2, (int)lote.Status);
        }

        [Fact]
        public void SiO2_EhOpcional_PodeSerNull()
        {
            // Arrange & Act
            var lote = new LoteMinerio
            {
                CodigoLote = "TEST",
                MinaOrigem = "TestMine",
                LocalizacaoAtual = "TestLoc",
                SiO2 = null
            };

            // Assert
            Assert.Null(lote.SiO2);
        }

        [Fact]
        public void P_EhOpcional_PodeSerNull()
        {
            // Arrange & Act
            var lote = new LoteMinerio
            {
                CodigoLote = "TEST",
                MinaOrigem = "TestMine",
                LocalizacaoAtual = "TestLoc",
                P = null
            };

            // Assert
            Assert.Null(lote.P);
        }

        [Fact]
        public void SiO2_PodeSerDefinidoEDepoisResetadoParaNull()
        {
            // Arrange
            var lote = new LoteMinerio
            {
                SiO2 = 5.5m
            };

            // Act
            lote.SiO2 = null;

            // Assert
            Assert.Null(lote.SiO2);
        }

        [Fact]
        public void P_PodeSerDefinidoEDepoisResetadoParaNull()
        {
            // Arrange
            var lote = new LoteMinerio
            {
                P = 0.05m
            };

            // Act
            lote.P = null;

            // Assert
            Assert.Null(lote.P);
        }
    }
}
