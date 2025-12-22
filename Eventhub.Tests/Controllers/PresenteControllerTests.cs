using Eventhub.Api.Controllers;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Eventhub.Tests.Controllers;

public class PresentesControllerTests
{
    private readonly Mock<IPresenteService> _serviceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PresentesController _controller;

    public PresentesControllerTests()
    {
        _serviceMock = new Mock<IPresenteService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _controller = new PresentesController(_serviceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ListarTodos_DeveRetornarOkComLista()
    {
        // Arrange
        var dtos = new List<PresenteDto> { new PresenteDto { Id = 1, Nome = "Notebook" } };
        _serviceMock.Setup(s => s.ListarTodosAsync()).ReturnsAsync(dtos);

        // Act
        var result = await _controller.ListarTodos();

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNotFound_SeNaoEncontrado()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync((PresenteDto?)null);

        // Act
        var result = await _controller.ObterPorId(1);

        // Assert
        var notFoundResult = result as ObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarOk_QuandoEncontrado()
    {
        // Arrange
        var dto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _serviceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync(dto);

        // Act
        var result = await _controller.ObterPorId(1);

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Criar_DeveRetornarCreated_ComPresente()
    {
        // Arrange
        var createDto = new CreatePresenteDto { Nome = "Notebook", Descricao = "Dell", Valor = 3000 };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _serviceMock.Setup(s => s.AdicionarAsync(createDto)).ReturnsAsync(presenteDto);

        // Act
        var result = await _controller.Criar(createDto);

        // Assert
        var createdResult = result as ObjectResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Atualizar_DeveRetornarBadRequest_QuandoIdNaoCorresponde()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 2, Nome = "Notebook" };

        // Act
        var result = await _controller.Atualizar(1, dto);

        // Assert
        var badRequestResult = result as ObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Atualizar_DeveRetornarOk_QuandoSucesso()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 1, Nome = "Notebook" };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _serviceMock.Setup(s => s.AtualizarAsync(dto)).ReturnsAsync(presenteDto);

        // Act
        var result = await _controller.Atualizar(1, dto);

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ListarTodos_ServiceLancaExcecao_DeveRetornarErro()
    {
        // Arrange
        _serviceMock.Setup(s => s.ListarTodosAsync()).ThrowsAsync(new Exception("erro"));

        // Act
        var result = await _controller.ListarTodos();

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }
}