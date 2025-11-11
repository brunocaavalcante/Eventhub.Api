using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class ConvidadosServiceTests
{
    private readonly Mock<IConvidadosRepository> _convidadosRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ConvidadosService _convidadosService;

    public ConvidadosServiceTests()
    {
        _convidadosRepositoryMock = new Mock<IConvidadosRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _convidadosService = new ConvidadosService(_convidadosRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarConvidado_QuandoDadosValidos()
    {
        // Arrange
        var convidado = new Convidados
        {
            Id = 1,
            IdEvento = 1,
            IdFoto = 1,
            Nome = "Maria Silva",
            Email = "maria@teste.com",
            Telefone = "11988888888"
        };

        _convidadosRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Convidados>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _convidadosService.AdicionarAsync(convidado);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Maria Silva");
        resultado.Email.Should().Be("maria@teste.com");
        _convidadosRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Convidados>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoNomeVazio()
    {
        // Arrange
        var convidado = new Convidados
        {
            IdEvento = 1,
            IdFoto = 1,
            Nome = "",
            Email = "maria@teste.com",
            Telefone = "11988888888"
        };

        // Act
        Func<Task> act = async () => await _convidadosService.AdicionarAsync(convidado);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*nome*");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoEmailInvalido()
    {
        // Arrange
        var convidado = new Convidados
        {
            IdEvento = 1,
            IdFoto = 1,
            Nome = "Maria Silva",
            Email = "emailinvalido",
            Telefone = "11988888888"
        };

        // Act
        Func<Task> act = async () => await _convidadosService.AdicionarAsync(convidado);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*inválido*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarConvidado_QuandoDadosValidos()
    {
        // Arrange
        var convidado = new Convidados
        {
            Id = 1,
            IdEvento = 1,
            IdFoto = 1,
            Nome = "Maria Silva Atualizada",
            Email = "maria@teste.com",
            Telefone = "11988888888"
        };

        _convidadosRepositoryMock.Setup(x => x.GetByIdAsync(convidado.Id))
            .ReturnsAsync(convidado);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _convidadosService.AtualizarAsync(convidado);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Maria Silva Atualizada");
        _convidadosRepositoryMock.Verify(x => x.GetByIdAsync(convidado.Id), Times.Once);
        _convidadosRepositoryMock.Verify(x => x.Update(It.IsAny<Convidados>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverConvidado_QuandoConvidadoExiste()
    {
        // Arrange
        var convidadoId = 1;
        var convidado = new Convidados
        {
            Id = convidadoId,
            IdEvento = 1,
            IdFoto = 1,
            Nome = "Maria Silva",
            Email = "maria@teste.com",
            Telefone = "11988888888"
        };

        _convidadosRepositoryMock.Setup(x => x.GetByIdAsync(convidadoId))
            .ReturnsAsync(convidado);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _convidadosService.RemoverAsync(convidadoId);

        // Assert
        _convidadosRepositoryMock.Verify(x => x.GetByIdAsync(convidadoId), Times.Once);
        _convidadosRepositoryMock.Verify(x => x.Remove(It.IsAny<Convidados>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarConvidados_QuandoEventoExiste()
    {
        // Arrange
        var eventoId = 1;
        var convidados = new List<Convidados>
        {
            new Convidados { Id = 1, IdEvento = eventoId, IdFoto = 1, Nome = "Convidado 1", Email = "c1@teste.com", Telefone = "11911111111" },
            new Convidados { Id = 2, IdEvento = eventoId, IdFoto = 1, Nome = "Convidado 2", Email = "c2@teste.com", Telefone = "11922222222" }
        };

        _convidadosRepositoryMock.Setup(x => x.GetByEventoAsync(eventoId))
            .ReturnsAsync(convidados);

        // Act
        var resultado = await _convidadosService.ObterPorEventoAsync(eventoId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(c => c.IdEvento == eventoId);
        _convidadosRepositoryMock.Verify(x => x.GetByEventoAsync(eventoId), Times.Once);
    }
}
