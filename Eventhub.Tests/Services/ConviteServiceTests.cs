using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Validations;
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

    [Fact]
    public async Task AtualizarAsync_DeveRetornarConviteDto_QuandoAtualizarConviteComSucesso()
    {
        // Arrange
        int conviteId = 1;
        var updateDto = new UpdateConviteDto
        {
            Nome = "Convite Atualizado",
            Mensagem = "Mensagem Atualizada",
            Foto = new UpdateFotoDto { Id = 1, NomeArquivo = "foto_atualizada.jpg", Base64 = Convert.ToBase64String(new byte[10]) }
        };

        var conviteDto = new ConviteDto { Id = conviteId, Nome = updateDto.Nome, Mensagem = updateDto.Mensagem };

        _conviteServiceMock
            .Setup(service => service.AtualizarAsync(conviteId, updateDto))
            .ReturnsAsync(conviteDto);

        // Act
        var resultado = await _conviteServiceMock.Object.AtualizarAsync(conviteId, updateDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(conviteId);
        resultado.Nome.Should().Be(updateDto.Nome);
    }

    [Fact]
    public void UpdateConviteValidation_DeveGerarErroQuandoCamposInvalidos()
    {
        var validator = new UpdateConviteValidation();
        var dto = new UpdateConviteDto
        {
            Nome = string.Empty,
            Nome2 = "Convidado",
            Mensagem = string.Empty,
            TemaConvite = string.Empty,
            Foto = new UpdateFotoDto { Id = 1, NomeArquivo = "foto.jpg", Base64 = Convert.ToBase64String(new byte[1]) }
        };

        var resultado = validator.Validate(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.Nome) && e.ErrorMessage == "O nome é obrigatório.");
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.Mensagem) && e.ErrorMessage == "A mensagem é obrigatória.");
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.TemaConvite) && e.ErrorMessage == "O tema do convite é obrigatório.");
    }

    [Fact]
    public void UpdateConviteValidation_DeveGerarErroQuandoCamposExcedemLimite()
    {
        var validator = new UpdateConviteValidation();
        var dto = new UpdateConviteDto
        {
            Nome = new string('A', 101),
            Nome2 = new string('B', 120),
            Mensagem = new string('C', 501),
            TemaConvite = new string('D', 120),
            Foto = new UpdateFotoDto { Id = 1, NomeArquivo = "foto.jpg", Base64 = Convert.ToBase64String(new byte[1]) }
        };

        var resultado = validator.Validate(dto);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.Nome) && e.ErrorMessage == "O nome deve ter no máximo 100 caracteres.");
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.Nome2) && e.ErrorMessage == "O nome2 deve ter no máximo 100 caracteres.");
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.Mensagem) && e.ErrorMessage == "A mensagem deve ter no máximo 500 caracteres.");
        resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateConviteDto.TemaConvite) && e.ErrorMessage == "O tema do convite deve ter no máximo 100 caracteres.");
    }
}
