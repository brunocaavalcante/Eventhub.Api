using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class ProgramacaoEventoServiceTests
{
    private readonly Mock<IProgramacaoEventoRepository> _programacaoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProgramacaoEventoService _programacaoService;

    public ProgramacaoEventoServiceTests()
    {
        _programacaoRepositoryMock = new Mock<IProgramacaoEventoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _programacaoService = new ProgramacaoEventoService(_programacaoRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarProgramacao_QuandoDadosValidos()
    {
        // Arrange
        var programacao = new ProgramacaoEvento
        {
            Id = 1,
            IdEvento = 1,
            Titulo = "Palestra de Abertura",
            Descricao = "Descrição da palestra",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(2),
            Local = "Auditório Principal",
            IdFoto = 1,
            IdStatus = 1,
            DataCadastro = DateTime.Now
        };

        _programacaoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ProgramacaoEvento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _programacaoService.AdicionarAsync(programacao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Palestra de Abertura");
        _programacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoTituloVazio()
    {
        // Arrange
        var programacao = new ProgramacaoEvento
        {
            IdEvento = 1,
            Titulo = "",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(2),
            IdFoto = 1,
            IdStatus = 1
        };

        // Act
        Func<Task> act = async () => await _programacaoService.AdicionarAsync(programacao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*título*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarProgramacao_QuandoDadosValidos()
    {
        // Arrange
        var programacao = new ProgramacaoEvento
        {
            Id = 1,
            IdEvento = 1,
            Titulo = "Palestra Atualizada",
            Descricao = "Descrição atualizada",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(3),
            Local = "Novo Local",
            IdFoto = 1,
            IdStatus = 1,
            DataCadastro = DateTime.Now
        };

        _programacaoRepositoryMock.Setup(x => x.GetByIdAsync(programacao.Id))
            .ReturnsAsync(programacao);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _programacaoService.AtualizarAsync(programacao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Palestra Atualizada");
        _programacaoRepositoryMock.Verify(x => x.GetByIdAsync(programacao.Id), Times.Once);
        _programacaoRepositoryMock.Verify(x => x.Update(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverProgramacao_QuandoProgramacaoExiste()
    {
        // Arrange
        var programacaoId = 1;
        var programacao = new ProgramacaoEvento
        {
            Id = programacaoId,
            IdEvento = 1,
            Titulo = "Palestra de Abertura",
            Data = DateTime.Now,
            Duracao = TimeSpan.FromHours(2),
            IdFoto = 1,
            IdStatus = 1
        };

        _programacaoRepositoryMock.Setup(x => x.GetByIdAsync(programacaoId))
            .ReturnsAsync(programacao);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _programacaoService.RemoverAsync(programacaoId);

        // Assert
        _programacaoRepositoryMock.Verify(x => x.GetByIdAsync(programacaoId), Times.Once);
        _programacaoRepositoryMock.Verify(x => x.Remove(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarProgramacoes_QuandoEventoExiste()
    {
        // Arrange
        var eventoId = 1;
        var programacoes = new List<ProgramacaoEvento>
        {
            new ProgramacaoEvento { Id = 1, IdEvento = eventoId, Titulo = "Programação 1", Data = DateTime.Now, Duracao = TimeSpan.FromHours(1), IdFoto = 1, IdStatus = 1 },
            new ProgramacaoEvento { Id = 2, IdEvento = eventoId, Titulo = "Programação 2", Data = DateTime.Now.AddHours(2), Duracao = TimeSpan.FromHours(1), IdFoto = 1, IdStatus = 1 }
        };

        _programacaoRepositoryMock.Setup(x => x.GetByEventoAsync(eventoId))
            .ReturnsAsync(programacoes);

        // Act
        var resultado = await _programacaoService.ObterPorEventoAsync(eventoId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(p => p.IdEvento == eventoId);
        _programacaoRepositoryMock.Verify(x => x.GetByEventoAsync(eventoId), Times.Once);
    }
}
