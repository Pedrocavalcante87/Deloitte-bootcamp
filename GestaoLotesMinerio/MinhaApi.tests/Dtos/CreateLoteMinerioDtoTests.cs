using System;
using MinhaApi.Dtos;
using Xunit;

namespace MinhaApi.Tests.Dtos
{
    public class CreateLoteMinerioDtoTests
    {
        [Fact]
        public void Construtor_Padrao_DeveInicializarStringsComoVaziasENullablesComoNull()
        {
            // Arrange & Act
            var dto = new CreateLoteMinerioDto();

            // Assert – strings com default = ""
            Assert.NotNull(dto.CodigoLote);
            Assert.Equal(string.Empty, dto.CodigoLote);

            Assert.NotNull(dto.MinaOrigem);
            Assert.Equal(string.Empty, dto.MinaOrigem);

            Assert.NotNull(dto.LocalizacaoAtual);
            Assert.Equal(string.Empty, dto.LocalizacaoAtual);

            // Assert – decimais e inteiros com default = 0
            Assert.Equal(0m, dto.TeorFe);
            Assert.Equal(0m, dto.Umidade);
            Assert.Equal(0m, dto.Toneladas);
            Assert.Equal(0, dto.Status);

            // Assert – campos opcionais começam como null
            Assert.Null(dto.SiO2);
            Assert.Null(dto.P);
            Assert.Null(dto.DataProducao);
        }

        [Fact]
        public void Set_Get_DeveAtribuirERecuperarValoresCorretamente()
        {
            // Arrange
            var data = new DateTime(2026, 2, 9, 10, 30, 0, DateTimeKind.Utc);

            var dto = new CreateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-000987",
                MinaOrigem = "Carajás N5",
                TeorFe = 65.2m,
                Umidade = 7.8m,
                SiO2 = 3.9m,
                P = 0.04m,
                Toneladas = 12000.5m,
                DataProducao = data,
                Status = 1, // EmTransporte
                LocalizacaoAtual = "EFVM - Trem 456"
            };

            // Act & Assert
            Assert.Equal("MNA-2026-000987", dto.CodigoLote);
            Assert.Equal("Carajás N5", dto.MinaOrigem);
            Assert.Equal(65.2m, dto.TeorFe);
            Assert.Equal(7.8m, dto.Umidade);
            Assert.Equal(3.9m, dto.SiO2);
            Assert.Equal(0.04m, dto.P);
            Assert.Equal(12000.5m, dto.Toneladas);
            Assert.Equal(data, dto.DataProducao);
            Assert.Equal(1, dto.Status);
            Assert.Equal("EFVM - Trem 456", dto.LocalizacaoAtual);
        }

        [Fact]
        public void DataProducao_PodeSerDefinidaEDepoisResetadaParaNull()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto();
            var data = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);

            // Act
            dto.DataProducao = data;

            // Assert
            Assert.Equal(data, dto.DataProducao);

            // Act novamente
            dto.DataProducao = null;

            // Assert
            Assert.Null(dto.DataProducao);
        }

        [Fact]
        public void SiO2_PodeSerDefinidoEDepoisResetadoParaNull()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto
            {
                SiO2 = 4.5m
            };

            // Act
            dto.SiO2 = null;

            // Assert
            Assert.Null(dto.SiO2);
        }

        [Fact]
        public void P_PodeSerDefinidoEDepoisResetadoParaNull()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto
            {
                P = 0.05m
            };

            // Act
            dto.P = null;

            // Assert
            Assert.Null(dto.P);
        }

        [Fact]
        public void Status_EmEstoque_DeveSerZero()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto
            {
                Status = 0
            };

            // Assert
            Assert.Equal(0, dto.Status);
        }

        [Fact]
        public void Status_EmTransporte_DeveSerUm()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto
            {
                Status = 1
            };

            // Assert
            Assert.Equal(1, dto.Status);
        }

        [Fact]
        public void Status_Embarcado_DeveSerDois()
        {
            // Arrange
            var dto = new CreateLoteMinerioDto
            {
                Status = 2
            };

            // Assert
            Assert.Equal(2, dto.Status);
        }

        [Fact]
        public void ValoresDecimais_DevemAceitarPrecisao()
        {
            // Arrange & Act
            var dto = new CreateLoteMinerioDto
            {
                TeorFe = 66.75m,
                Umidade = 8.125m,
                SiO2 = 4.055m,
                P = 0.031m,
                Toneladas = 15432.875m
            };

            // Assert
            Assert.Equal(66.75m, dto.TeorFe);
            Assert.Equal(8.125m, dto.Umidade);
            Assert.Equal(4.055m, dto.SiO2);
            Assert.Equal(0.031m, dto.P);
            Assert.Equal(15432.875m, dto.Toneladas);
        }
    }
}
