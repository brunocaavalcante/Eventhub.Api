using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class TipoEventoServiceTests
{
    private readonly Mock<ITipoEventoRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TipoEventoService _service;

    public TipoEventoServiceTests()
    {
        _repoMock = new Mock<ITipoEventoRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new TipoEventoService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ListarTodosAsync_DeveRetornarListaDeDtos()
    {
        // Arrange
        var entidades = new List<TipoEvento> { new TipoEvento { Id = 1, Nome = "Teste", Icon = "icon", Descricao = "desc", IdFoto = 2 } };
        var dtos = new List<TipoEventoDto> { new TipoEventoDto { Id = 1, Nome = "Teste", Icon = "icon", Descricao = "desc", IdFoto = 2 } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entidades);
        _mapperMock.Setup(m => m.Map<IEnumerable<TipoEventoDto>>(entidades)).Returns(dtos);

        // Act
        var result = await _service.ListarTodosAsync();

        // Assert
        result.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task ListarTodosAsync_ListaVazia_DeveRetornarListaVazia()
    {
        // Arrange
        var entidades = new List<TipoEvento>();
        var dtos = new List<TipoEventoDto>();
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entidades);
        _mapperMock.Setup(m => m.Map<IEnumerable<TipoEventoDto>>(entidades)).Returns(dtos);

        // Act
        var result = await _service.ListarTodosAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
