using System.Threading.Tasks;
using Eventhub.Api.Controllers;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Eventhub.Tests.Controllers;

public class ContribuicaoPresenteControllerTests
{
    private readonly Mock<IContribuicaoPresenteService> _serviceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly ContribuicaoPresenteController _controller;

    public ContribuicaoPresenteControllerTests()
    {
        _controller = new ContribuicaoPresenteController(_serviceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Cancelar_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Duplicidade" };
        _serviceMock.Setup(s => s.CancelarAsync(dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Cancelar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Cancelar_DeveRetornarErro_QuandoServiceLancaExcecao()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Motivo" };
        _serviceMock.Setup(s => s.CancelarAsync(dto)).ThrowsAsync(new System.Exception("erro"));

        // Act
        var result = await _controller.Cancelar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

      [Fact]
    public async Task CriarContribuicao_DeveRetornarCreated_ComContribuicao()
    {
        // Arrange
        var dto = new CreateContribuicaoPresenteDto
        {
            IdPresente = 1,
            IdParticipante = 2,
            Valor = 50,
            FormaPagamento = "Pix",
            Comprovante = new UploadFotoDto
            {
                NomeArquivo = "comprovante.jpg",
                Base64 = Convert.ToBase64String(new byte[10]),
                TipoImagem = "image/jpeg"
            }
        };

        var contribuicaoDto = new ContribuicaoPresenteDto { Id = 1, IdPresente = 1, Valor = 50 };
        _serviceMock.Setup(s => s.CriarAsync(dto)).ReturnsAsync(contribuicaoDto);

        // Act
        var result = await _controller.CriarContribuicao(dto);

        // Assert
        var createdResult = result as ObjectResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task CriarContribuicao_QuandoServiceLancaExcecao_DeveRetornarErro()
    {
        // Arrange
        var dto = new CreateContribuicaoPresenteDto
        {
            IdPresente = 1,
            IdParticipante = 2,
            Valor = 50,
            FormaPagamento = "Pix",
            Comprovante = new UploadFotoDto
            {
                NomeArquivo = "comprovante.jpg",
                Base64 = Convert.ToBase64String(new byte[10]),
                TipoImagem = "image/jpeg"
            }
        };

        _serviceMock.Setup(s => s.CriarAsync(dto)).ThrowsAsync(new Exception("erro"));

        // Act
        var result = await _controller.CriarContribuicao(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Confirmar_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        _serviceMock.Setup(s => s.ConfirmarAsync(dto)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Confirmar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(200);
        _serviceMock.Verify(s => s.ConfirmarAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Confirmar_DeveRetornarErro_QuandoServiceLancaExcecao()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        _serviceMock.Setup(s => s.ConfirmarAsync(dto)).ThrowsAsync(new Exception("Erro ao confirmar"));

        // Act
        var result = await _controller.Confirmar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Confirmar_DeveRetornarErro_QuandoContribuicaoNaoEncontrada()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 999, IdPresente = 10 };
        _serviceMock.Setup(s => s.ConfirmarAsync(dto))
            .ThrowsAsync(new Domain.Exceptions.ExceptionValidation("Contribuição não encontrada."));

        // Act
        var result = await _controller.Confirmar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Confirmar_DeveRetornarErro_QuandoContribuicaoJaConfirmada()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        _serviceMock.Setup(s => s.ConfirmarAsync(dto))
            .ThrowsAsync(new Domain.Exceptions.ExceptionValidation("A contribuição já está confirmada."));

        // Act
        var result = await _controller.Confirmar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Confirmar_DeveRetornarErro_QuandoIdInvalido()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 0, IdPresente = 0 };
        _serviceMock.Setup(s => s.ConfirmarAsync(dto))
            .ThrowsAsync(new Domain.Exceptions.ExceptionValidation("Id da contribuição ou do presente inválido."));

        // Act
        var result = await _controller.Confirmar(dto);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(400);
    }

}
