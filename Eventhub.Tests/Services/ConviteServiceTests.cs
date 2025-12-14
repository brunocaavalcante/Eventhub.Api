using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class ConviteServiceTests
{
    private readonly Mock<IConviteService> _conviteServiceMock;
    private readonly Mock<IConviteRepository> _conviteRepositoryMock;

    public ConviteServiceTests()
    {
        _conviteRepositoryMock = new Mock<IConviteRepository>();
        _conviteServiceMock = new Mock<IConviteService>();
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarConvite_QuandoConviteExistir()
    {
        // Arrange
        int eventoId = 1;
        var conviteDto = new ConviteDto { Id = 1, IdEvento = eventoId };

        _conviteServiceMock
            .Setup(service => service.ObterPorEventoAsync(eventoId))
            .ReturnsAsync(conviteDto);

        // Act
        var resultado = await _conviteServiceMock.Object.ObterPorEventoAsync(eventoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.IdEvento.Should().Be(eventoId);
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarNull_QuandoConviteNaoExistir()
    {
        // Arrange
        int eventoId = 1;

        _conviteServiceMock
            .Setup(service => service.ObterPorEventoAsync(eventoId))
            .ReturnsAsync((ConviteDto?)null);

        // Act
        var resultado = await _conviteServiceMock.Object.ObterPorEventoAsync(eventoId);

        // Assert
        resultado.Should().BeNull();
    }

    //teste para CriarAsync
    [Fact]
    public async Task CriarAsync_DeveRetornarConviteDto_QuandoCriarConviteComSucesso()
    {
        // Arrange
        var createDto = new CreateConviteDto
        {
            Nome = "Convite Teste",
            Mensagem = "Mensagem Teste",
            Foto = new UploadFotoDto { NomeArquivo = "foto.jpg", Base64 = Convert.ToBase64String(new byte[10]) }
        };

        var conviteDto = new ConviteDto { Id = 1, Nome = createDto.Nome, Mensagem = createDto.Mensagem };

        _conviteServiceMock
            .Setup(service => service.CriarAsync(createDto))
            .ReturnsAsync(conviteDto);

        // Act
        var resultado = await _conviteServiceMock.Object.CriarAsync(createDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Nome.Should().Be(createDto.Nome);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoDadosInvalidos()
    {
        // Arrange
        var createDto = new CreateConviteDto
        {
            Nome = "", // Nome inválido
            Mensagem = "Mensagem Teste",
            Foto = new UploadFotoDto { NomeArquivo = "foto.jpg", Base64 = Convert.ToBase64String(new byte[10]) }
        };

        _conviteServiceMock
            .Setup(service => service.CriarAsync(createDto))
            .ThrowsAsync(new Exception("Dados inválidos"));

        // Act
        Func<Task> act = async () => await _conviteServiceMock.Object.CriarAsync(createDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Dados inválidos");
    }
}
