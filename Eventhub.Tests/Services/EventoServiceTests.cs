using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using AutoMapper;
using Eventhub.Tests.Helpers;

namespace Eventhub.Tests.Services;

public class EventoServiceTests
{
    private readonly Mock<IEventoRepository> _eventoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EventoService _eventoService;
    private readonly Mock<IFotosService> _fotoService;
    private readonly Mock<IParticipanteService> _participanteService;
    private readonly Mock<IStatusEventoRepository> _statusEventoRepositoryMock;
    private readonly IMapper _mapper;

    public EventoServiceTests()
    {
        _eventoRepositoryMock = new Mock<IEventoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fotoService = new Mock<IFotosService>();
        _participanteService = new Mock<IParticipanteService>();
        _statusEventoRepositoryMock = new Mock<IStatusEventoRepository>();
        _mapper = AutoMapperHelper.CreateMapper();
        _eventoService = new EventoService(_eventoRepositoryMock.Object, _unitOfWorkMock.Object, _mapper, _fotoService.Object, _participanteService.Object, _statusEventoRepositoryMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarEventoDto_QuandoEventoExiste()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            Descricao = "Descrição do evento",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(1)
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);

        // Act
        var resultado = await _eventoService.ObterPorIdAsync(eventoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(eventoId);
        resultado.Nome.Should().Be("Evento Teste");
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarEvento_QuandoDadosValidos()
    {
        // Arrange
        var evento = new EventoCadastroDto
        {
            Nome = "Evento Teste",
            IdTipoEvento = 1,
            IdUsuarioCriador = 1,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Descricao = "Descrição do evento",
            MaxConvidado = 100,
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new(),
            Participantes = new()
        };

        _eventoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _eventoService.AdicionarAsync(evento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Descrição do evento");
        _eventoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoDescricaoVazia()
    {
        // Arrange
        var evento = new EventoCadastroDto
        {
            Nome = "Evento Teste",
            IdTipoEvento = 1,
            IdUsuarioCriador = 1,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Descricao = "",
            MaxConvidado = 100,
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new(),
            Participantes = new()
        };

        // Act
        Func<Task> act = async () => await _eventoService.AdicionarAsync(evento);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>();
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoDataFimMenorQueDataInicio()
    {
        // Arrange
        var evento = new EventoCadastroDto
        {
            Nome = "Evento Teste",
            IdTipoEvento = 1,
            IdUsuarioCriador = 1,
            DataInicio = DateTime.Now.AddDays(2),
            DataFim = DateTime.Now.AddDays(1), // Data fim menor que início
            Descricao = "Evento Teste",
            MaxConvidado = 100,
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new(),
            Participantes = new()
        };

        // Act
        Func<Task> act = async () => await _eventoService.AdicionarAsync(evento);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*data de fim*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarEvento_QuandoDadosValidos()
    {
        // Arrange
        var evento = new Evento
        {
            Id = 1,
            IdTipoEvento = 1,
            IdStatus = 1,
            IdEndereco = 1,
            IdUsuarioCriador = 1,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Nome = "Evento Teste",
            Descricao = "Descrição atualizada",
            MaxConvidado = 150,
            DataInclusao = DateTime.Now
        };

        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _eventoService.AtualizarAsync(evento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Descrição atualizada");
        _eventoRepositoryMock.Verify(x => x.Update(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverEvento_QuandoEventoExiste()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Descricao = "Evento Teste",
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(1)
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _eventoService.RemoverAsync(eventoId);

        // Assert
        _eventoRepositoryMock.Verify(x => x.GetByIdAsync(eventoId), Times.Once);
        _eventoRepositoryMock.Verify(x => x.Remove(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoEventoNaoExiste()
    {
        // Arrange
        var eventoId = 999;
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync((Evento?)null);

        // Act
        Func<Task> act = async () => await _eventoService.RemoverAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }
}
