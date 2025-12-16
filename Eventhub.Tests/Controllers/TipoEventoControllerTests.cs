using Eventhub.Api.Controllers;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace Eventhub.Tests.Controllers;

public class TipoEventoControllerTests
{
    private readonly Mock<ITipoEventoService> _serviceMock;
    private readonly TipoEventoController _controller;

    public TipoEventoControllerTests()
    {
        _serviceMock = new Mock<ITipoEventoService>();
        _controller = new TipoEventoController(_serviceMock.Object);
    }

    [Fact]
    public async Task ListarTodos_DeveRetornarOkComLista()
    {
        // Arrange
        var dtos = new List<TipoEventoDto> { new TipoEventoDto { Id = 1, Nome = "Teste", Icon = "icon", Descricao = "desc", IdFoto = 2 } };
        _serviceMock.Setup(s => s.ListarTodosAsync()).ReturnsAsync(dtos);

        // Act
        var result = await _controller.ListarTodos();

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
