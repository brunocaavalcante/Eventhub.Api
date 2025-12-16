using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class NotificacaoServiceTests
{
    private readonly Mock<INotificacaoRepository> _notificacaoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly NotificacaoService _notificacaoService;

    public NotificacaoServiceTests()
    {
        _notificacaoRepositoryMock = new Mock<INotificacaoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _notificacaoService = new NotificacaoService(_notificacaoRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarNotificacao_QuandoDadosValidos()
    {
        // Arrange
        var notificacao = new Notificacao
        {
            Id = 1,
            IdEvento = 1,
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            Titulo = "Nova Notificação",
            Descricao = "Descrição da notificação",
            Status = "Pendente",
            Prioridade = 3,
            Data = DateTime.Now,
            DataCadastro = DateTime.Now
        };

        _notificacaoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Notificacao>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _notificacaoService.AdicionarAsync(notificacao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Nova Notificação");
        resultado.Prioridade.Should().Be(3);
        _notificacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Notificacao>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoTituloVazio()
    {
        // Arrange
        var notificacao = new Notificacao
        {
            IdEvento = 1,
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            Titulo = "",
            Descricao = "Descrição",
            Status = "Pendente",
            Prioridade = 3
        };

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(notificacao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*título*");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoPrioridadeInvalida()
    {
        // Arrange
        var notificacao = new Notificacao
        {
            IdEvento = 1,
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            Titulo = "Título",
            Descricao = "Descrição",
            Status = "Pendente",
            Prioridade = 10 // Prioridade fora do range 1-5
        };

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(notificacao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*prioridade*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarNotificacao_QuandoDadosValidos()
    {
        // Arrange
        var notificacao = new Notificacao
        {
            Id = 1,
            IdEvento = 1,
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            Titulo = "Notificação Atualizada",
            Descricao = "Descrição atualizada",
            Status = "Lida",
            Prioridade = 5,
            Data = DateTime.Now
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(notificacao.Id))
            .ReturnsAsync(notificacao);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _notificacaoService.AtualizarAsync(notificacao);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be("Lida");
        _notificacaoRepositoryMock.Verify(x => x.GetByIdAsync(notificacao.Id), Times.Once);
        _notificacaoRepositoryMock.Verify(x => x.Update(It.IsAny<Notificacao>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorUsuarioAsync_DeveRetornarNotificacoes_QuandoUsuarioExiste()
    {
        // Arrange
        var usuarioId = 2;
        var notificacoes = new List<Notificacao>
        {
            new Notificacao { Id = 1, IdUsuarioDestino = usuarioId, Titulo = "Notificação 1", Descricao = "Desc 1", Status = "Pendente", Prioridade = 1, Data = DateTime.Now, IdEvento = 1, IdUsuarioOrigem = 1 },
            new Notificacao { Id = 2, IdUsuarioDestino = usuarioId, Titulo = "Notificação 2", Descricao = "Desc 2", Status = "Lida", Prioridade = 2, Data = DateTime.Now, IdEvento = 1, IdUsuarioOrigem = 1 }
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByUsuarioDestinoAsync(usuarioId))
            .ReturnsAsync(notificacoes);

        // Act
        var resultado = await _notificacaoService.ObterPorUsuarioAsync(usuarioId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(n => n.IdUsuarioDestino == usuarioId);
        _notificacaoRepositoryMock.Verify(x => x.GetByUsuarioDestinoAsync(usuarioId), Times.Once);
    }

    [Fact]
    public async Task ObterNaoLidasAsync_DeveRetornarApenasNotificacoesNaoLidas()
    {
        // Arrange
        var usuarioId = 2;
        var notificacoesNaoLidas = new List<Notificacao>
        {
            new Notificacao { Id = 1, IdUsuarioDestino = usuarioId, Titulo = "Notificação 1", Descricao = "Desc 1", Status = "Pendente", Prioridade = 1, Data = DateTime.Now, IdEvento = 1, IdUsuarioOrigem = 1 }
        };

        _notificacaoRepositoryMock.Setup(x => x.GetNaoLidasByUsuarioAsync(usuarioId))
            .ReturnsAsync(notificacoesNaoLidas);

        // Act
        var resultado = await _notificacaoService.ObterNaoLidasAsync(usuarioId);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.Should().OnlyContain(n => n.Status == "Pendente");
        _notificacaoRepositoryMock.Verify(x => x.GetNaoLidasByUsuarioAsync(usuarioId), Times.Once);
    }
}
