using AutoMapper;
using Eventhub.Application.DTOs;
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
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProgramacaoEventoService _programacaoService;

    public ProgramacaoEventoServiceTests()
    {
        _programacaoRepositoryMock = new Mock<IProgramacaoEventoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _programacaoService = new ProgramacaoEventoService(
            _programacaoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarProgramacao_QuandoDadosValidos()
    {
        // Arrange
        var dto = new ProgramacaoEventoCreateDto
        {
            IdEvento = 1,
            Titulo = "Palestra de Abertura",
            Descricao = "Descrição da palestra",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(2),
            Local = "Auditório Principal",
            Responsavel = "João Silva",
            IdStatus = 1
        };

        var programacao = new ProgramacaoEvento
        {
            Id = 1,
            IdEvento = dto.IdEvento,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Data = dto.Data,
            Duracao = dto.Duracao ?? TimeSpan.FromHours(1),
            Local = dto.Local,
            Responsavel = dto.Responsavel,
            IdStatus = dto.IdStatus,
            DataCadastro = DateTime.UtcNow,
            Evento = new Evento { Id = 1, Nome = "Evento Teste" },
            Status = new StatusProgramacao { Id = 1, Descricao = "Ativo" }
        };

        var responseDto = new ProgramacaoEventoResponseDto
        {
            Id = programacao.Id,
            IdEvento = programacao.IdEvento,
            NomeEvento = "Evento Teste",
            Titulo = programacao.Titulo,
            Descricao = programacao.Descricao,
            Data = programacao.Data,
            Duracao = programacao.Duracao,
            Local = programacao.Local,
            Responsavel = programacao.Responsavel,
            IdStatus = programacao.IdStatus,
            DescricaoStatus = "Ativo",
            DataCadastro = programacao.DataCadastro
        };

        _mapperMock.Setup(x => x.Map<ProgramacaoEvento>(dto)).Returns(programacao);
        _mapperMock.Setup(x => x.Map<ProgramacaoEventoResponseDto>(It.IsAny<ProgramacaoEvento>())).Returns(responseDto);
        _programacaoRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ProgramacaoEvento>())).Returns(Task.CompletedTask);
        _programacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(It.IsAny<int>())).ReturnsAsync(programacao);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _programacaoService.AdicionarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Palestra de Abertura");
        resultado.NomeEvento.Should().Be("Evento Teste");
        _programacaoRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoTituloVazio()
    {
        // Arrange
        var dto = new ProgramacaoEventoCreateDto
        {
            IdEvento = 1,
            Titulo = "",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(2),
            IdStatus = 1
        };

        var programacao = new ProgramacaoEvento
        {
            IdEvento = dto.IdEvento,
            Titulo = dto.Titulo,
            Data = dto.Data,
            Duracao = dto.Duracao ?? TimeSpan.FromHours(1),
            IdStatus = dto.IdStatus
        };

        _mapperMock.Setup(x => x.Map<ProgramacaoEvento>(dto)).Returns(programacao);

        // Act
        Func<Task> act = async () => await _programacaoService.AdicionarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*título*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarProgramacao_QuandoDadosValidos()
    {
        // Arrange
        var dto = new ProgramacaoEventoUpdateDto
        {
            Id = 1,
            Titulo = "Palestra Atualizada",
            Descricao = "Descrição atualizada",
            Data = DateTime.Now.AddDays(1),
            Duracao = TimeSpan.FromHours(3),
            Local = "Novo Local",
            Responsavel = "Maria Santos",
            IdStatus = 1
        };

        var programacaoExistente = new ProgramacaoEvento
        {
            Id = dto.Id,
            IdEvento = 1,
            Titulo = "Palestra Antiga",
            Descricao = "Descrição antiga",
            Data = DateTime.Now,
            Duracao = TimeSpan.FromHours(2),
            Local = "Local Antigo",
            Responsavel = "João Silva",
            IdStatus = 1,
            DataCadastro = DateTime.UtcNow.AddDays(-1),
            Evento = new Evento { Id = 1, Nome = "Evento Teste" },
            Status = new StatusProgramacao { Id = 1, Descricao = "Ativo" }
        };

        var responseDto = new ProgramacaoEventoResponseDto
        {
            Id = dto.Id,
            IdEvento = programacaoExistente.IdEvento,
            NomeEvento = "Evento Teste",
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Data = dto.Data,
            Duracao = (TimeSpan)dto.Duracao,
            Local = dto.Local,
            Responsavel = dto.Responsavel,
            IdStatus = dto.IdStatus,
            DescricaoStatus = "Ativo",
            DataCadastro = programacaoExistente.DataCadastro
        };

        _programacaoRepositoryMock.Setup(x => x.GetByIdAsync(dto.Id)).ReturnsAsync(programacaoExistente);
        _mapperMock.Setup(x => x.Map(dto, programacaoExistente)).Returns(programacaoExistente);
        _programacaoRepositoryMock.Setup(x => x.GetByIdWithIncludesAsync(dto.Id)).ReturnsAsync(programacaoExistente);
        _mapperMock.Setup(x => x.Map<ProgramacaoEventoResponseDto>(It.IsAny<ProgramacaoEvento>())).Returns(responseDto);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var resultado = await _programacaoService.AtualizarAsync(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Titulo.Should().Be("Palestra Atualizada");
        resultado.Local.Should().Be("Novo Local");
        _programacaoRepositoryMock.Verify(x => x.GetByIdAsync(dto.Id), Times.Once);
        _programacaoRepositoryMock.Verify(x => x.Update(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
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
            Titulo = "Programação a ser removida",
            Data = DateTime.Now,
            Duracao = TimeSpan.FromHours(1),
            IdStatus = 1
        };

        _programacaoRepositoryMock.Setup(x => x.GetByIdAsync(programacaoId)).ReturnsAsync(programacao);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _programacaoService.RemoverAsync(programacaoId);

        // Assert
        _programacaoRepositoryMock.Verify(x => x.GetByIdAsync(programacaoId), Times.Once);
        _programacaoRepositoryMock.Verify(x => x.Remove(It.IsAny<ProgramacaoEvento>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoProgramacaoNaoExiste()
    {
        // Arrange
        var programacaoId = 999;
        _programacaoRepositoryMock.Setup(x => x.GetByIdAsync(programacaoId)).ReturnsAsync((ProgramacaoEvento?)null);

        // Act
        Func<Task> act = async () => await _programacaoService.RemoverAsync(programacaoId);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarProgramacoes()
    {
        // Arrange
        var idEvento = 1;
        var programacoes = new List<ProgramacaoEvento>
        {
            new ProgramacaoEvento
            {
                Id = 1,
                IdEvento = idEvento,
                Titulo = "Programação 1",
                Data = DateTime.Now,
                Duracao = TimeSpan.FromHours(1),
                Evento = new Evento { Id = idEvento, Nome = "Evento Teste" },
                Status = new StatusProgramacao { Id = 1, Descricao = "Ativo" }
            },
            new ProgramacaoEvento
            {
                Id = 2,
                IdEvento = idEvento,
                Titulo = "Programação 2",
                Data = DateTime.Now.AddHours(2),
                Duracao = TimeSpan.FromHours(1),
                Evento = new Evento { Id = idEvento, Nome = "Evento Teste" },
                Status = new StatusProgramacao { Id = 1, Descricao = "Ativo" }
            }
        };

        var responseDtos = programacoes.Select(p => new ProgramacaoEventoResponseDto
        {
            Id = p.Id,
            IdEvento = p.IdEvento,
            NomeEvento = "Evento Teste",
            Titulo = p.Titulo,
            Data = p.Data,
            Duracao = p.Duracao,
            IdStatus = 1,
            DescricaoStatus = "Ativo"
        }).ToList();

        _programacaoRepositoryMock.Setup(x => x.GetByEventoAsync(idEvento)).ReturnsAsync(programacoes);
        _mapperMock.Setup(x => x.Map<IEnumerable<ProgramacaoEventoResponseDto>>(programacoes)).Returns(responseDtos);

        // Act
        var resultado = await _programacaoService.ObterPorEventoAsync(idEvento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Titulo.Should().Be("Programação 1");
        _programacaoRepositoryMock.Verify(x => x.GetByEventoAsync(idEvento), Times.Once);
    }
}
