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
    private readonly Mock<IPerfilEventoPermissaoService> _perfilEventoPermissaoServiceMock;
    private readonly Mock<IPerfilRepository> _perfilRepositoryMock;
    private readonly Mock<IPresenteRepository> _presenteRepositoryMock;
    private readonly Mock<IContribuicaoPresenteRepository> _contribuicaoPresenteRepositoryMock;

    public EventoServiceTests()
    {
        _eventoRepositoryMock = new Mock<IEventoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fotoService = new Mock<IFotosService>();
        _participanteService = new Mock<IParticipanteService>();
        _statusEventoRepositoryMock = new Mock<IStatusEventoRepository>();
        _perfilEventoPermissaoServiceMock = new Mock<IPerfilEventoPermissaoService>();
        _perfilRepositoryMock = new Mock<IPerfilRepository>();
        _presenteRepositoryMock = new Mock<IPresenteRepository>();
        _contribuicaoPresenteRepositoryMock = new Mock<IContribuicaoPresenteRepository>();
        _mapper = AutoMapperHelper.CreateMapper();

        _eventoService = new EventoService(
            _eventoRepositoryMock.Object, 
            _unitOfWorkMock.Object, 
            _mapper, 
            _fotoService.Object, 
            _participanteService.Object, 
            _statusEventoRepositoryMock.Object, 
            _perfilEventoPermissaoServiceMock.Object, 
            _perfilRepositoryMock.Object,
            _presenteRepositoryMock.Object,
            _contribuicaoPresenteRepositoryMock.Object);
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
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _eventoService.AdicionarAsync(evento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Descrição do evento");
        _eventoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
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
        var eventoDto = new Eventhub.Application.DTOs.Evento.UpdateEventoDto
        {
            Id = 1,
            Nome = "Evento Teste Atualizado",
            Descricao = "Descrição atualizada",
            IdTipoEvento = 1,
            MaxConvidado = 150,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Atualizada",
                Numero = "456",
                Cidade = "Cidade Teste",
                Cep = "98765-432"
            },
            Imagens = new()
        };

        var eventoExistente = new Evento
        {
            Id = 1,
            IdTipoEvento = 1,
            IdStatus = 1,
            IdEndereco = 1,
            IdUsuarioCriador = 1,
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddDays(1),
            Nome = "Evento Teste",
            Descricao = "Descrição antiga",
            MaxConvidado = 100,
            DataInclusao = DateTime.Now
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoDto.Id))
            .ReturnsAsync(eventoExistente);
        _eventoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _eventoService.AtualizarAsync(eventoDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Descricao.Should().Be("Descrição atualizada");
        resultado.Nome.Should().Be("Evento Teste Atualizado");
        _eventoRepositoryMock.Verify(x => x.GetByIdAsync(eventoDto.Id), Times.Once);
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoEventoNaoEncontrado()
    {
        // Arrange
        var eventoDto = new Eventhub.Application.DTOs.Evento.UpdateEventoDto
        {
            Id = 999,
            Nome = "Evento Teste",
            Descricao = "Descrição",
            IdTipoEvento = 1,
            MaxConvidado = 100,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new()
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoDto.Id))
            .ReturnsAsync((Evento?)null);

        // Act
        Func<Task> act = async () => await _eventoService.AtualizarAsync(eventoDto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
        _eventoRepositoryMock.Verify(x => x.GetByIdAsync(eventoDto.Id), Times.Once);
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoDataFimMenorQueDataInicio()
    {
        // Arrange
        var eventoDto = new Eventhub.Application.DTOs.Evento.UpdateEventoDto
        {
            Id = 1,
            Nome = "Evento Teste",
            Descricao = "Descrição",
            IdTipoEvento = 1,
            MaxConvidado = 100,
            DataInicio = DateTime.Now.AddDays(2),
            DataFim = DateTime.Now.AddDays(1), // Data fim menor que início
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new()
        };

        // Act
        Func<Task> act = async () => await _eventoService.AtualizarAsync(eventoDto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*data de término*posterior*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoNomeVazio()
    {
        // Arrange
        var eventoDto = new Eventhub.Application.DTOs.Evento.UpdateEventoDto
        {
            Id = 1,
            Nome = "", // Nome vazio
            Descricao = "Descrição",
            IdTipoEvento = 1,
            MaxConvidado = 100,
            DataInicio = DateTime.Now.AddDays(1),
            DataFim = DateTime.Now.AddDays(2),
            Endereco = new EnderecoEventoDto
            {
                Logradouro = "Rua Teste",
                Numero = "123",
                Cidade = "Cidade Teste",
                Cep = "12345-678"
            },
            Imagens = new()
        };

        // Act
        Func<Task> act = async () => await _eventoService.AtualizarAsync(eventoDto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*nome*obrigatório*");
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

    [Fact]
    public async Task ObterEventosPorUsuarioAsync_DeveRetornarEventosCriadosEParticipacoes_SemDuplicatas()
    {
        // Arrange
        var idUsuario = 1;
        var hoje = DateTime.Now;

        var eventosCriados = new List<Evento>
        {
            new Evento 
            { 
                Id = 1, 
                IdUsuarioCriador = idUsuario, 
                Nome = "Evento Criado 1",
                Descricao = "Evento criado pelo usuário",
                DataInicio = hoje.AddDays(5),
                DataFim = hoje.AddDays(6),
                IdTipoEvento = 1,
                IdStatus = 1,
                TipoEvento = new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 },
                Status = new StatusEvento { Id = 1, Descricao = "Ativo" },
                Endereco = new EnderecoEvento { Id = 1, Logradouro = "Rua A", Numero = "123", Cep = "12345-678", Cidade = "São Paulo" }
            },
            new Evento 
            { 
                Id = 3, 
                IdUsuarioCriador = idUsuario, 
                Nome = "Evento Criado E Participante",
                Descricao = "Evento criado e participa",
                DataInicio = hoje.AddDays(15),
                DataFim = hoje.AddDays(16),
                IdTipoEvento = 1,
                IdStatus = 1,
                TipoEvento = new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 },
                Status = new StatusEvento { Id = 1, Descricao = "Ativo" },
                Endereco = new EnderecoEvento { Id = 1, Logradouro = "Rua C", Numero = "789", Cep = "12345-678", Cidade = "São Paulo" }
            }
        };

        var eventosParticipante = new List<Evento>
        {
            new Evento 
            { 
                Id = 2, 
                IdUsuarioCriador = 2, 
                Nome = "Evento Participante 1",
                Descricao = "Evento onde é participante",
                DataInicio = hoje.AddDays(10),
                DataFim = hoje.AddDays(11),
                IdTipoEvento = 1,
                IdStatus = 1,
                TipoEvento = new TipoEvento { Id = 1, Descricao = "Festa", IdFoto = 1 },
                Status = new StatusEvento { Id = 1, Descricao = "Ativo" },
                Endereco = new EnderecoEvento { Id = 1, Logradouro = "Rua B", Numero = "456", Cep = "12345-678", Cidade = "Rio de Janeiro" }
            },
            new Evento 
            { 
                Id = 3, 
                IdUsuarioCriador = idUsuario, 
                Nome = "Evento Criado E Participante",
                Descricao = "Evento criado e participa",
                DataInicio = hoje.AddDays(15),
                DataFim = hoje.AddDays(16),
                IdTipoEvento = 1,
                IdStatus = 1,
                TipoEvento = new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 },
                Status = new StatusEvento { Id = 1, Descricao = "Ativo" },
                Endereco = new EnderecoEvento { Id = 1, Logradouro = "Rua C", Numero = "789", Cep = "12345-678", Cidade = "São Paulo" }
            }
        };

        _eventoRepositoryMock.Setup(x => x.GetEventosByUsuarioAsync(idUsuario))
            .ReturnsAsync(eventosCriados);
        _eventoRepositoryMock.Setup(x => x.GetEventosByParticipanteUsuarioAsync(idUsuario))
            .ReturnsAsync(eventosParticipante);

        // Act
        var resultado = (await _eventoService.ObterEventosPorUsuarioAsync(idUsuario)).ToList();

        // Assert
        resultado.Should().HaveCount(3); // 3 eventos únicos (1 criado, 1 participante, 1 ambos)
        resultado.Should().Contain(e => e.Id == 1); // Evento criado
        resultado.Should().Contain(e => e.Id == 2); // Evento participante
        resultado.Should().Contain(e => e.Id == 3); // Evento criado E participante (sem duplicata)
        
        // Verificar ordenação por DataInicio
        resultado[0].DataInicio.Should().BeBefore(resultado[1].DataInicio);
        resultado[1].DataInicio.Should().BeBefore(resultado[2].DataInicio);
        
        // Verificar que ambos os métodos do repositório foram chamados
        _eventoRepositoryMock.Verify(x => x.GetEventosByUsuarioAsync(idUsuario), Times.Once);
        _eventoRepositoryMock.Verify(x => x.GetEventosByParticipanteUsuarioAsync(idUsuario), Times.Once);
    }

    #region Testes CancelarEventoAsync

    [Fact]
    public async Task CancelarEventoAsync_DeveCancelarEvento_QuandoNaoHouverContribuicoesProblematicas()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 1, // Ativo
            DataInicio = DateTime.UtcNow.AddDays(10)
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(new List<Presente>());
        _eventoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        evento.IdStatus.Should().Be(4); // Cancelado
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveLancarExcecao_QuandoEventoNaoEncontrado()
    {
        // Arrange
        var eventoId = 999;
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync((Evento?)null);

        // Act
        Func<Task> act = async () => await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveLancarExcecao_QuandoEventoJaEstaCancelado()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 4 // Já cancelado
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);

        // Act
        Func<Task> act = async () => await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já está cancelado*");
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveLancarExcecao_QuandoEventoJaConcluido()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 5 // Concluído
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);

        // Act
        Func<Task> act = async () => await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já foi concluído*");
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveLancarExcecao_QuandoHouverContribuicoesConfirmadas()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 1
        };

        var presentes = new List<Presente>
        {
            new Presente { Id = 1, IdEvento = eventoId }
        };

        var contribuicoesConfirmadas = new List<ContribuicaoPresente>
        {
            new ContribuicaoPresente { Id = 1, IdPresente = 1, IdStatusContribuicao = 2, Valor = 100 }
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(presentes);
        _contribuicaoPresenteRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ContribuicaoPresente, bool>>>()))
            .ReturnsAsync(contribuicoesConfirmadas);

        // Act
        Func<Task> act = async () => await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*contribuição(ões) confirmada(s)*");
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveLancarExcecao_QuandoHouverContribuicoesEmAnalise()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 1
        };

        var presentes = new List<Presente>
        {
            new Presente { Id = 1, IdEvento = eventoId }
        };

        var contribuicoesEmAnalise = new List<ContribuicaoPresente>
        {
            new ContribuicaoPresente { Id = 1, IdPresente = 1, IdStatusContribuicao = 4, Valor = 50 }
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(presentes);
        
        // Setup para retornar lista vazia primeiro (confirmadas) e depois as em análise
        var callCount = 0;
        _contribuicaoPresenteRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ContribuicaoPresente, bool>>>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1 ? new List<ContribuicaoPresente>() : contribuicoesEmAnalise;
            });

        // Act
        Func<Task> act = async () => await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*contribuição(ões) em análise*");
    }

    [Fact]
    public async Task CancelarEventoAsync_DeveCancelarContribuicoesPendentes_ELiberarReservas()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 1
        };

        var presentes = new List<Presente>
        {
            new Presente { Id = 1, IdEvento = eventoId, IdStatus = 2, IdParticipanteReservou = 10 } // Reservado
        };

        var contribuicoesPendentes = new List<ContribuicaoPresente>
        {
            new ContribuicaoPresente { Id = 1, IdPresente = 1, IdStatusContribuicao = 1, Valor = 50 }
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(presentes);
        
        var callCount = 0;
        _contribuicaoPresenteRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ContribuicaoPresente, bool>>>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount <= 2 ? new List<ContribuicaoPresente>() : contribuicoesPendentes;
            });
        
        _contribuicaoPresenteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ContribuicaoPresente>()))
            .Returns(Task.CompletedTask);
        _presenteRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Presente>()))
            .Returns(Task.CompletedTask);
        _eventoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _eventoService.CancelarEventoAsync(eventoId);

        // Assert
        evento.IdStatus.Should().Be(4); // Cancelado
        _contribuicaoPresenteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ContribuicaoPresente>()), Times.Once);
        _presenteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Presente>()), Times.Once);
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Once);
    }

    #endregion

    #region Testes ReativarEventoAsync

    [Fact]
    public async Task ReativarEventoAsync_DeveReativarEvento_QuandoEventoCanceladoEDataFutura()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 4, // Cancelado
            DataInicio = DateTime.UtcNow.AddDays(10),
            DataFim = DateTime.UtcNow.AddDays(11)
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(new List<Presente>());
        _eventoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        evento.IdStatus.Should().Be(3); // Agendado
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ReativarEventoAsync_DeveLancarExcecao_QuandoEventoNaoEncontrado()
    {
        // Arrange
        var eventoId = 999;
        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync((Evento?)null);

        // Act
        Func<Task> act = async () => await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task ReativarEventoAsync_DeveLancarExcecao_QuandoEventoNaoEstaCancelado()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 1 // Ativo
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);

        // Act
        Func<Task> act = async () => await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Apenas eventos cancelados*");
    }

    [Fact]
    public async Task ReativarEventoAsync_DeveLancarExcecao_QuandoDataJaPassou()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 4, // Cancelado
            DataInicio = DateTime.UtcNow.AddDays(-5),
            DataFim = DateTime.UtcNow.AddDays(-4) // Evento já terminou
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);

        // Act
        Func<Task> act = async () => await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já foi concluído*");
    }

    [Fact]
    public async Task ReativarEventoAsync_DeveLancarExcecao_QuandoHouverContribuicoesEstornadas()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 4, // Cancelado
            DataInicio = DateTime.UtcNow.AddDays(10),
            DataFim = DateTime.UtcNow.AddDays(11)
        };

        var presentes = new List<Presente>
        {
            new Presente { Id = 1, IdEvento = eventoId }
        };

        var contribuicoesEstornadas = new List<ContribuicaoPresente>
        {
            new ContribuicaoPresente { Id = 1, IdPresente = 1, IdStatusContribuicao = 5, Valor = 100 }
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(presentes);
        _contribuicaoPresenteRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ContribuicaoPresente, bool>>>()))
            .ReturnsAsync(contribuicoesEstornadas);

        // Act
        Func<Task> act = async () => await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*contribuição(ões) já estornada(s)*");
    }

    [Fact]
    public async Task ReativarEventoAsync_DeveDefinirStatusAtivo_QuandoDataJaIniciou()
    {
        // Arrange
        var eventoId = 1;
        var evento = new Evento
        {
            Id = eventoId,
            Nome = "Evento Teste",
            IdStatus = 4, // Cancelado
            DataInicio = DateTime.UtcNow.AddHours(-2), // Já começou
            DataFim = DateTime.UtcNow.AddHours(2) // Mas ainda não terminou
        };

        _eventoRepositoryMock.Setup(x => x.GetByIdAsync(eventoId))
            .ReturnsAsync(evento);
        _presenteRepositoryMock.Setup(x => x.GetByEventIdAsync(eventoId))
            .ReturnsAsync(new List<Presente>());
        _eventoRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Evento>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _eventoService.ReativarEventoAsync(eventoId);

        // Assert
        evento.IdStatus.Should().Be(1); // Ativo
        _eventoRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Evento>()), Times.Once);
    }

    #endregion
}
