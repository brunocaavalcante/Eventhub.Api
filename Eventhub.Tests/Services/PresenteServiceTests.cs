using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class PresenteServiceTests
{
    private readonly Mock<IPresenteRepository> _repoMock;
    private readonly Mock<IFotosService> _fotosServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PresenteService _service;

    public PresenteServiceTests()
    {
        _repoMock = new Mock<IPresenteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fotosServiceMock = new Mock<IFotosService>();
        _mapperMock = new Mock<IMapper>();
        _service = new PresenteService(_repoMock.Object, _fotosServiceMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarPresente_ComSucesso()
    {
        // Arrange
        var dto = new CreatePresenteDto { Nome = "Notebook", Descricao = "Dell", Valor = 3000 };
        var presente = new Presente { Id = 1, Nome = "Notebook", Descricao = "Dell", Valor = 3000 };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };

        _mapperMock.Setup(m => m.Map<Presente>(dto)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Presente>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AdicionarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("Notebook");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Presente>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.AtualizarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.RemoverAsync(1);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarDto_QuandoEncontrado()
    {
        // Arrange
        var presente = new Presente { Id = 1, Nome = "Notebook" };
        var dto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(dto);

        // Act
        var result = await _service.ObterPorIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task ListarTodosAsync_DeveRetornarListaDeDtos()
    {
        // Arrange
        var presentes = new List<Presente> { new Presente { Id = 1, Nome = "Notebook" } };
        var dtos = new List<PresenteDto> { new PresenteDto { Id = 1, Nome = "Notebook" } };
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(presentes);
        _mapperMock.Setup(m => m.Map<IEnumerable<PresenteDto>>(presentes)).Returns(dtos);

        // Act
        var result = await _service.ListarTodosAsync();

        // Assert
        result.Should().HaveCount(1);
    }
}