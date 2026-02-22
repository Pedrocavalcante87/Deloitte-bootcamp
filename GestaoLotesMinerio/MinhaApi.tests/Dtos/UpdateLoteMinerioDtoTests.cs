using System;
using MinhaApi.Dtos;
using Xunit;

namespace MinhaApi.Tests.Dtos
{
    public class UpdateLoteMinerioDtoTests
    {
        [Fact]
        public void Construtor_Padrao_DeveInicializarStringsComoVaziasENullablesComoNull()
        {
            // Arrange & Act
            var dto = new UpdateLoteMinerioDto();

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
            var data = new DateTime(2026, 2, 10, 14, 45, 0, DateTimeKind.Utc);

            var dto = new UpdateLoteMinerioDto
            {
                CodigoLote = "MNA-2026-999999",
                MinaOrigem = "Carajás N8",
                TeorFe = 68.3m,
                Umidade = 6.5m,
                SiO2 = 2.8m,
                P = 0.02m,
                Toneladas = 18500.25m,
                DataProducao = data,
                Status = 2, // Embarcado
                LocalizacaoAtual = "Porto Tubarão"
            };

            // Act & Assert
            Assert.Equal("MNA-2026-999999", dto.CodigoLote);
            Assert.Equal("Carajás N8", dto.MinaOrigem);
            Assert.Equal(68.3m, dto.TeorFe);
            Assert.Equal(6.5m, dto.Umidade);
            Assert.Equal(2.8m, dto.SiO2);
            Assert.Equal(0.02m, dto.P);
            Assert.Equal(18500.25m, dto.Toneladas);
            Assert.Equal(data, dto.DataProducao);
            Assert.Equal(2, dto.Status);
            Assert.Equal("Porto Tubarão", dto.LocalizacaoAtual);
        }

        [Fact]
        public void DataProducao_PodeSerDefinidaEDepoisResetadaParaNull()
        {
            // Arrange
            var dto = new UpdateLoteMinerioDto();
            var data = new DateTime(2026, 1, 20, 12, 0, 0, DateTimeKind.Utc);

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
            var dto = new UpdateLoteMinerioDto
            {
                SiO2 = 5.2m
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
            var dto = new UpdateLoteMinerioDto
            {
                P = 0.06m
            };

            // Act
            dto.P = null;

            // Assert
            Assert.Null(dto.P);
        }

        [Fact]
        public void Status_PodeSerAlterado()
        {
            // Arrange
            var dto = new UpdateLoteMinerioDto
            {
                Status = 0 // EmEstoque
            };

            // Act
            dto.Status = 1; // EmTransporte

            // Assert
            Assert.Equal(1, dto.Status);

            // Act
            dto.Status = 2; // Embarcado

            // Assert
            Assert.Equal(2, dto.Status);
        }

        [Fact]
        public void CodigoLote_PodeSerAlterado()
        {
            // Arrange
            var dto = new UpdateLoteMinerioDto
            {
                CodigoLote = "CODIGO-ANTIGO"
            };

            // Act
            dto.CodigoLote = "CODIGO-NOVO";

            // Assert
            Assert.Equal("CODIGO-NOVO", dto.CodigoLote);
        }

        [Fact]
        public void ValoresDecimais_DevemAceitarPrecisao()
        {
            // Arrange & Act
            var dto = new UpdateLoteMinerioDto
            {
                TeorFe = 67.89m,
                Umidade = 7.654m,
                SiO2 = 3.210m,
                P = 0.045m,
                Toneladas = 19876.543m
            };

            // Assert
            Assert.Equal(67.89m, dto.TeorFe);
            Assert.Equal(7.654m, dto.Umidade);
            Assert.Equal(3.210m, dto.SiO2);
            Assert.Equal(0.045m, dto.P);
            Assert.Equal(19876.543m, dto.Toneladas);
        }
    }
}
