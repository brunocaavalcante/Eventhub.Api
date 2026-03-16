using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class NotificacaoServiceTests
{
    private readonly Mock<INotificacaoRepository> _notificacaoRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IEventoRepository> _eventoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly NotificacaoService _notificacaoService;

    public NotificacaoServiceTests()
    {
        _notificacaoRepositoryMock = new Mock<INotificacaoRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _eventoRepositoryMock = new Mock<IEventoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _notificacaoService = new NotificacaoService(
            _notificacaoRepositoryMock.Object,
            _usuarioRepositoryMock.Object,
            _eventoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    #region AdicionarAsync Tests

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarNotificacao_QuandoDadosValidos()
    {
        // Arrange
        var dto = new NotificacaoCreateDto
        {
            Titulo = "Nova Notificação",
            Descricao = "Descrição da notificação",
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            IdEvento = 1,
            Status = EnumNotificacaoStatus.Enviada,
            Prioridade = EnumNotificacaoPrioridade.Alta
        };

        var usuarioOrigem = new Usuario { Id = 1, Nome = "Usuario1" };
        var usuarioDestino = new Usuario { Id = 2, Nome = "Usuario2" };
        var evento = new Evento { Id = 1, Nome = "Evento1" };

        var notificacao = new Notificacao
        {
            Id = 1,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            IdUsuarioOrigem = dto.IdUsuarioOrigem,
            IdUsuarioDestino = dto.IdUsuarioDestino,
            IdEvento = dto.IdEvento,
            Status = dto.Status,
            Prioridade = dto.Prioridade,
            DataCadastro = DateTime.UtcNow,
            DataEnvio = DateTime.UtcNow,
            DataLeitura = DateTime.MinValue,
            Data = DateTime.UtcNow
        };

        var notificacaoComIncludes = new Notificacao
        {
            Id = 1,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            UsuarioOrigem = usuarioOrigem,
            UsuarioDestino = usuarioDestino,
            Evento = evento
        };

        var responseDto = new NotificacaoResponseDto
        {
            Id = 1,
            Titulo = "Nova Notificação",
            Descricao = "Descrição da notificação",
            NomeUsuarioOrigem = "Usuario1",
            NomeUsuarioDestino = "Usuario2",
            NomeEvento = "Evento1"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioOrigem)).ReturnsAsync(usuarioOrigem);
        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioDestino)).ReturnsAsync(usuarioDestino);
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdEvento)).ReturnsAsync(evento);
        _mapperMock.Setup(x => x.Map<Notificacao>(dto)).Returns(notificacao);
        _notificacaoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Notificacao>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);
        _notificacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(It.IsAny<int>())).ReturnsAsync(notificacaoComIncludes);
        _mapperMock.Setup(x => x.Map<NotificacaoResponseDto>(notificacaoComIncludes)).Returns(responseDto);

        // Act
        var resultado = await _notificacaoService.AdicionarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Nova Notificação");
        resultado.NomeUsuarioOrigem.Should().Be("Usuario1");
        resultado.NomeUsuarioDestino.Should().Be("Usuario2");
        resultado.NomeEvento.Should().Be("Evento1");
        _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(dto.IdUsuarioOrigem), Times.Once);
        _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(dto.IdUsuarioDestino), Times.Once);
        _eventoRepositoryMock.Verify(x => x.GetByIdAsync(dto.IdEvento), Times.Once);
        _notificacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Notificacao>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoUsuarioOrigemNaoEncontrado()
    {
        // Arrange
        var dto = new NotificacaoCreateDto
        {
            Titulo = "Nova Notificação",
            IdUsuarioOrigem = 999,
            IdUsuarioDestino = 2,
            IdEvento = 1
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioOrigem))
            .ReturnsAsync((Usuario?)null);

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Usuário de origem não encontrado.");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoUsuarioDestinoNaoEncontrado()
    {
        // Arrange
        var dto = new NotificacaoCreateDto
        {
            Titulo = "Nova Notificação",
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 999,
            IdEvento = 1
        };

        var usuarioOrigem = new Usuario { Id = 1, Nome = "Usuario1" };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioOrigem))
            .ReturnsAsync(usuarioOrigem);
        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioDestino))
            .ReturnsAsync((Usuario?)null);

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Usuário de destino não encontrado.");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoEventoNaoEncontrado()
    {
        // Arrange
        var dto = new NotificacaoCreateDto
        {
            Titulo = "Nova Notificação",
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            IdEvento = 999
        };

        var usuarioOrigem = new Usuario { Id = 1, Nome = "Usuario1" };
        var usuarioDestino = new Usuario { Id = 2, Nome = "Usuario2" };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioOrigem))
            .ReturnsAsync(usuarioOrigem);
        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioDestino))
            .ReturnsAsync(usuarioDestino);
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdEvento))
            .ReturnsAsync((Evento?)null);

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Evento não encontrado.");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoTituloVazio()
    {
        // Arrange
        var dto = new NotificacaoCreateDto
        {
            Titulo = "",
            Descricao = "Descrição válida",
            IdUsuarioOrigem = 1,
            IdUsuarioDestino = 2,
            IdEvento = 1
        };

        var usuarioOrigem = new Usuario { Id = 1, Nome = "Usuario1" };
        var usuarioDestino = new Usuario { Id = 2, Nome = "Usuario2" };
        var evento = new Evento { Id = 1, Nome = "Evento1" };

        var notificacao = new Notificacao
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioOrigem))
            .ReturnsAsync(usuarioOrigem);
        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdUsuarioDestino))
            .ReturnsAsync(usuarioDestino);
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(dto.IdEvento))
            .ReturnsAsync(evento);
        _mapperMock.Setup(x => x.Map<Notificacao>(dto))
            .Returns(notificacao);

        // Act
        Func<Task> act = async () => await _notificacaoService.AdicionarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*título*");
    }

    #endregion

    #region RemoverAsync Tests

    [Fact]
    public async Task RemoverAsync_DeveRemoverNotificacao_QuandoUsuarioTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuario = 2;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuario,
            Titulo = "Notificação para remover",
            DataCadastro = DateTime.UtcNow.AddDays(1)
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync(notificacao);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _notificacaoService.RemoverAsync(idNotificacao, idUsuario);

        // Assert
        _notificacaoRepositoryMock.Verify(x => x.GetByIdAsync(idNotificacao), Times.Once);
        _notificacaoRepositoryMock.Verify(x => x.Remove(notificacao), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoNotificacaoNaoEncontrada()
    {
        // Arrange
        var idNotificacao = 999;
        var idUsuario = 2;

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync((Notificacao?)null);

        // Act
        Func<Task> act = async () => await _notificacaoService.RemoverAsync(idNotificacao, idUsuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Notificação não encontrada.");
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoUsuarioNaoTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuarioDestino = 2;
        var idUsuarioSemPermissao = 3;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuarioDestino,
            Titulo = "Notificação privada"
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync(notificacao);

        // Act
        Func<Task> act = async () => await _notificacaoService.RemoverAsync(idNotificacao, idUsuarioSemPermissao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Você não tem permissão para remover esta notificação.");
    }

    #endregion

    #region ObterPorIdAsync Tests

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNotificacao_QuandoUsuarioTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuario = 2;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuario,
            Titulo = "Notificação teste"
        };

        var responseDto = new NotificacaoResponseDto
        {
            Id = idNotificacao,
            Titulo = "Notificação teste",
            IdUsuarioDestino = idUsuario
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(idNotificacao))
            .ReturnsAsync(notificacao);
        _mapperMock.Setup(x => x.Map<NotificacaoResponseDto>(notificacao))
            .Returns(responseDto);

        // Act
        var resultado = await _notificacaoService.ObterPorIdAsync(idNotificacao, idUsuario);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(idNotificacao);
        resultado.Titulo.Should().Be("Notificação teste");
        _notificacaoRepositoryMock.Verify(x => x.GetByIdWithIncludesAsync(idNotificacao), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarNull_QuandoNotificacaoNaoEncontrada()
    {
        // Arrange
        var idNotificacao = 999;
        var idUsuario = 2;

        _notificacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(idNotificacao))
            .ReturnsAsync((Notificacao?)null);

        // Act
        var resultado = await _notificacaoService.ObterPorIdAsync(idNotificacao, idUsuario);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveLancarExcecao_QuandoUsuarioNaoTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuarioDestino = 2;
        var idUsuarioSemPermissao = 3;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuarioDestino,
            Titulo = "Notificação privada"
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(idNotificacao))
            .ReturnsAsync(notificacao);

        // Act
        Func<Task> act = async () => await _notificacaoService.ObterPorIdAsync(idNotificacao, idUsuarioSemPermissao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Você não tem permissão para acessar esta notificação.");
    }

    #endregion

    #region ObterPorUsuarioAsync Tests

    [Fact]
    public async Task ObterPorUsuarioAsync_DeveRetornarNotificacoes_QuandoUsuarioExiste()
    {
        // Arrange
        var idUsuario = 2;
        var notificacoes = new List<Notificacao>
        {
            new Notificacao { Id = 1, IdUsuarioDestino = idUsuario, Titulo = "Notificação 1", Status = EnumNotificacaoStatus.Enviada },
            new Notificacao { Id = 2, IdUsuarioDestino = idUsuario, Titulo = "Notificação 2", Status = EnumNotificacaoStatus.Lida }
        };

        var responseDtos = new List<NotificacaoResponseDto>
        {
            new NotificacaoResponseDto { Id = 1, IdUsuarioDestino = idUsuario, Titulo = "Notificação 1", Status = EnumNotificacaoStatus.Enviada },
            new NotificacaoResponseDto { Id = 2, IdUsuarioDestino = idUsuario, Titulo = "Notificação 2", Status = EnumNotificacaoStatus.Lida }
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByUsuarioDestinoAsync(idUsuario))
            .ReturnsAsync(notificacoes);
        _mapperMock.Setup(x => x.Map<IEnumerable<NotificacaoResponseDto>>(notificacoes))
            .Returns(responseDtos);

        // Act
        var resultado = await _notificacaoService.ObterPorUsuarioAsync(idUsuario);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(n => n.IdUsuarioDestino == idUsuario);
        _notificacaoRepositoryMock.Verify(x => x.GetByUsuarioDestinoAsync(idUsuario), Times.Once);
    }

    [Fact]
    public async Task ObterPorUsuarioAsync_DeveRetornarListaVazia_QuandoUsuarioNaoTemNotificacoes()
    {
        // Arrange
        var idUsuario = 2;
        var notificacoes = new List<Notificacao>();
        var responseDtos = new List<NotificacaoResponseDto>();

        _notificacaoRepositoryMock.Setup(x => x.GetByUsuarioDestinoAsync(idUsuario))
            .ReturnsAsync(notificacoes);
        _mapperMock.Setup(x => x.Map<IEnumerable<NotificacaoResponseDto>>(notificacoes))
            .Returns(responseDtos);

        // Act
        var resultado = await _notificacaoService.ObterPorUsuarioAsync(idUsuario);

        // Assert
        resultado.Should().BeEmpty();
    }

    #endregion

    #region ObterNaoLidasAsync Tests

    [Fact]
    public async Task ObterNaoLidasAsync_DeveRetornarApenasNotificacoesNaoLidas()
    {
        // Arrange
        var idUsuario = 2;
        var notificacoesNaoLidas = new List<Notificacao>
        {
            new Notificacao { Id = 1, IdUsuarioDestino = idUsuario, Titulo = "Notificação 1", Status = EnumNotificacaoStatus.Enviada },
            new Notificacao { Id = 2, IdUsuarioDestino = idUsuario, Titulo = "Notificação 2", Status = EnumNotificacaoStatus.Enviada }
        };

        var responseDtos = new List<NotificacaoResponseDto>
        {
            new NotificacaoResponseDto { Id = 1, IdUsuarioDestino = idUsuario, Titulo = "Notificação 1", Status = EnumNotificacaoStatus.Enviada },
            new NotificacaoResponseDto { Id = 2, IdUsuarioDestino = idUsuario, Titulo = "Notificação 2", Status = EnumNotificacaoStatus.Enviada }
        };

        _notificacaoRepositoryMock.Setup(x => x.GetNaoLidasByUsuarioAsync(idUsuario))
            .ReturnsAsync(notificacoesNaoLidas);
        _mapperMock.Setup(x => x.Map<IEnumerable<NotificacaoResponseDto>>(notificacoesNaoLidas))
            .Returns(responseDtos);

        // Act
        var resultado = await _notificacaoService.ObterNaoLidasAsync(idUsuario);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(n => n.Status == EnumNotificacaoStatus.Enviada);
        resultado.Should().OnlyContain(n => n.IdUsuarioDestino == idUsuario);
        _notificacaoRepositoryMock.Verify(x => x.GetNaoLidasByUsuarioAsync(idUsuario), Times.Once);
    }

    #endregion

    #region MarcarComoLidaAsync Tests

    [Fact]
    public async Task MarcarComoLidaAsync_DeveMarcarComoLida_QuandoUsuarioTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuario = 2;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuario,
            Titulo = "Notificação",
            Status = EnumNotificacaoStatus.Enviada,
            DataLeitura = DateTime.MinValue
        };

        var notificacaoAtualizada = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuario,
            Titulo = "Notificação",
            Status = EnumNotificacaoStatus.Lida,
            DataLeitura = DateTime.UtcNow
        };

        var responseDto = new NotificacaoResponseDto
        {
            Id = idNotificacao,
            Titulo = "Notificação",
            Status = EnumNotificacaoStatus.Lida
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync(notificacao);
        _notificacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(idNotificacao))
            .ReturnsAsync(notificacaoAtualizada);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(x => x.Map<NotificacaoResponseDto>(notificacaoAtualizada))
            .Returns(responseDto);

        // Act
        var resultado = await _notificacaoService.MarcarComoLidaAsync(idNotificacao, idUsuario);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Status.Should().Be(EnumNotificacaoStatus.Lida);
        notificacao.Status.Should().Be(EnumNotificacaoStatus.Lida);
        notificacao.DataLeitura.Should().NotBe(DateTime.MinValue);
        _notificacaoRepositoryMock.Verify(x => x.Update(notificacao), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarcarComoLidaAsync_DeveLancarExcecao_QuandoNotificacaoNaoEncontrada()
    {
        // Arrange
        var idNotificacao = 999;
        var idUsuario = 2;

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync((Notificacao?)null);

        // Act
        Func<Task> act = async () => await _notificacaoService.MarcarComoLidaAsync(idNotificacao, idUsuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Notificação não encontrada.");
    }

    [Fact]
    public async Task MarcarComoLidaAsync_DeveLancarExcecao_QuandoUsuarioNaoTemPermissao()
    {
        // Arrange
        var idNotificacao = 1;
        var idUsuarioDestino = 2;
        var idUsuarioSemPermissao = 3;

        var notificacao = new Notificacao
        {
            Id = idNotificacao,
            IdUsuarioDestino = idUsuarioDestino,
            Titulo = "Notificação privada",
            Status = EnumNotificacaoStatus.Enviada
        };

        _notificacaoRepositoryMock.Setup(x => x.GetByIdAsync(idNotificacao))
            .ReturnsAsync(notificacao);

        // Act
        Func<Task> act = async () => await _notificacaoService.MarcarComoLidaAsync(idNotificacao, idUsuarioSemPermissao);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("Você não tem permissão para marcar esta notificação como lida.");
    }

    #endregion

    #region MarcarTodasComoLidasAsync Tests

    [Fact]
    public async Task MarcarTodasComoLidasAsync_DeveMarcarTodasNotificacoes_QuandoChamado()
    {
        // Arrange
        var idUsuario = 2;

        _notificacaoRepositoryMock.Setup(x => x.MarcarTodasComoLidasAsync(idUsuario))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _notificacaoService.MarcarTodasComoLidasAsync(idUsuario);

        // Assert
        _notificacaoRepositoryMock.Verify(x => x.MarcarTodasComoLidasAsync(idUsuario), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion
}
