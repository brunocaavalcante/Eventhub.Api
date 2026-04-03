using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class ContribuicaoPresenteServiceTests
{
    private readonly Mock<IContribuicaoPresenteRepository> _contribuicaoRepoMock;
    private readonly Mock<IPresenteRepository> _presenteRepoMock;
    private readonly Mock<IFotosService> _fotosServiceMock;
    private readonly Mock<IParticipanteRepository> _participanteRepositoryMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IEventoRepository> _eventoRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ContribuicaoPresenteService _service;

    public ContribuicaoPresenteServiceTests()
    {
        _contribuicaoRepoMock = new Mock<IContribuicaoPresenteRepository>();
        _presenteRepoMock = new Mock<IPresenteRepository>();
        _fotosServiceMock = new Mock<IFotosService>();
        _participanteRepositoryMock = new Mock<IParticipanteRepository>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _eventoRepositoryMock = new Mock<IEventoRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new ContribuicaoPresenteService(
            _contribuicaoRepoMock.Object,
            _presenteRepoMock.Object,
            _fotosServiceMock.Object,
            _participanteRepositoryMock.Object,
            _usuarioRepositoryMock.Object,
            _eventoRepositoryMock.Object,
            _emailServiceMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task CriarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        var dto = CriarDtoValido();
        _presenteRepoMock.Setup(r => r.GetByIdAsync(dto.IdPresente)).ReturnsAsync((Presente?)null);

        Func<Task> act = async () => await _service.CriarAsync(dto);

        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Presente não encontrado*");
    }

    [Fact]
    public async Task CriarAsync_DeveCriarContribuicao_ComStatusEmAnalise()
    {
        var dto = CriarDtoValido();
        var presente = new Presente { Id = dto.IdPresente, Valor = 100m, IdStatus = (int)StatusPresenteEnum.Disponivel };
        var fotoDto = new FotoDto { Id = 10 };
        var contribuicao = new ContribuicaoPresente();
        var contribuicaoDto = new ContribuicaoPresenteDto();

        _presenteRepoMock.Setup(r => r.GetByIdAsync(dto.IdPresente)).ReturnsAsync(presente);
        _fotosServiceMock.Setup(f => f.UploadAsync(dto.Comprovante)).ReturnsAsync(fotoDto);
        _mapperMock.Setup(m => m.Map<ContribuicaoPresente>(dto)).Returns(contribuicao);
        _mapperMock.Setup(m => m.Map<ContribuicaoPresenteDto>(contribuicao)).Returns(contribuicaoDto);
        _contribuicaoRepoMock.Setup(r => r.AddAsync(contribuicao)).Returns(Task.CompletedTask);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(dto.IdPresente)).ReturnsAsync(50m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.CriarAsync(dto);

        result.Should().NotBeNull();
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.EmAnalise);
        contribuicao.IdFoto.Should().Be(fotoDto.Id);
        _contribuicaoRepoMock.Verify(r => r.AddAsync(contribuicao), Times.Once);
    }

    private static CreateContribuicaoPresenteDto CriarDtoValido()
    {
        return new CreateContribuicaoPresenteDto
        {
            IdPresente = 1,
            IdParticipante = 2,
            Valor = 50m,
            FormaPagamento = "Pix",
            Comprovante = new UploadFotoDto
            {
                NomeArquivo = "comprovante.jpg",
                Base64 = Convert.ToBase64String(new byte[10]),
                TipoImagem = "image/jpeg"
            }
        };
    }

    [Fact]
    public async Task CancelarAsync_DeveCancelarComJustificativa_EManterStatusPresente()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Duplicidade" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdParticipante = 100,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 300m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            IdEvento = 50,
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.EmArrecadacao 
        };
        var participante = new Participante { Id = 100, IdUsuario = 200 };
        var usuario = new Usuario { Id = 200, Nome = "João Silva", Email = "joao@test.com" };
        var evento = new Evento { Id = 50, Nome = "Casamento João e Maria" };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(500m); // Ainda tem contribuições
        _participanteRepositoryMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync(participante);
        _usuarioRepositoryMock.Setup(r => r.GetByIdAsync(200)).ReturnsAsync(usuario);
        _eventoRepositoryMock.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(evento);
        _emailServiceMock.Setup(e => e.EnviarEmailContribuicaoCanceladaAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        contribuicao.Justificativa.Should().Be("Duplicidade");
        _contribuicaoRepoMock.Verify(r => r.Update(contribuicao), Times.Once);
        _emailServiceMock.Verify(e => e.EnviarEmailContribuicaoCanceladaAsync(
            "joao@test.com", "João Silva", It.IsAny<string>(), 
            "Casamento João e Maria", 300m, "Duplicidade"), Times.Once);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.EmArrecadacao); // Status continua
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once); // Apenas uma vez (sem mudança no presente)
    }

    [Fact]
    public async Task CancelarAsync_DeveLancarExcecao_SeNaoEncontrado()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 99, Justificativa = "Motivo" };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ContribuicaoPresente?)null);

        // Act
        var act = async () => await _service.CancelarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>().WithMessage("*não encontrada*");
    }

    [Fact]
    public async Task CancelarAsync_DeveLancarExcecao_SeJaCancelada()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Motivo" };
        var entity = new ContribuicaoPresente { Id = 1, IdStatusContribuicao = (int)StatusContribuicaoEnum.Cancelado };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        var act = async () => await _service.CancelarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>().WithMessage("*já está cancelada*");
    }

    [Fact]
    public async Task CancelarAsync_DeveAlterarStatusPresenteParaDisponivel_QuandoTotalFicaZero()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Cancelamento total" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 500m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.EmArrecadacao 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(0m); // Total fica zero
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.Disponivel);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2)); // Uma para contribuição, outra para presente
    }

    [Fact]
    public async Task CancelarAsync_DeveAlterarStatusPresenteParaEmArrecadacao_QuandoTotalParcial()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Erro ao contribuir" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 600m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Reservado // Estava reservado
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(400m); // Total < valor do presente
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.EmArrecadacao);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task CancelarAsync_DeveManterStatusReservado_QuandoTotalAindaAtingeValor()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Duplicidade" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 200m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Reservado 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(1000m); // Total ainda >= valor
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.Reservado); // Mantém reservado
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Never); // Não atualiza o presente
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once); // Apenas uma vez para contribuição
    }

    [Fact]
    public async Task CancelarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Erro" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 999, // Presente inexistente
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Presente?)null);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(999)).ReturnsAsync(0m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var act = async () => await _service.CancelarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>().WithMessage("*Presente não encontrado*");
    }

    [Fact]
    public async Task CancelarAsync_DeveAtualizarApenasPresenteQuandoStatusMuda()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Teste" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 100m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Reservado 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(200m); // Status deve mudar para EmArrecadacao
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.EmArrecadacao);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task CancelarAsync_DeveAlterarStatusDeReservadoParaDisponivel()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Último cancelamento" };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado,
            Valor = 1000m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Reservado 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(0m); // Sem contribuições restantes
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.Disponivel);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task ConfirmarAsync_DeveConfirmarContribuicao_QuandoValido()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise,
            Valor = 500m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Disponivel 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(500m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.ConfirmarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Confirmado);
        _contribuicaoRepoMock.Verify(r => r.Update(contribuicao), Times.Once);
    }

    [Fact]
    public async Task ConfirmarAsync_DeveAtualizarStatusPresente_ParaEmArrecadacao_QuandoParcial()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise,
            Valor = 300m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.Disponivel 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(300m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.ConfirmarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Confirmado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.EmArrecadacao);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task ConfirmarAsync_DeveAtualizarStatusPresente_ParaReservado_QuandoAtingeValorTotal()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise,
            Valor = 700m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.EmArrecadacao 
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(1000m); // Total completo
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.ConfirmarAsync(dto);

        // Assert
        contribuicao.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Confirmado);
        presente.IdStatus.Should().Be((int)StatusPresenteEnum.Reservado);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }

    [Fact]
    public async Task ConfirmarAsync_DeveLancarExcecao_QuandoIdContribuicaoInvalido()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 0, IdPresente = 10 };

        // Act
        var act = async () => await _service.ConfirmarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Id da contribuição ou do presente inválido*");
    }

    [Fact]
    public async Task ConfirmarAsync_DeveLancarExcecao_QuandoIdPresenteInvalido()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 0 };

        // Act
        var act = async () => await _service.ConfirmarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Id da contribuição ou do presente inválido*");
    }

    [Fact]
    public async Task ConfirmarAsync_DeveLancarExcecao_QuandoContribuicaoNaoEncontrada()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 999, IdPresente = 10 };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ContribuicaoPresente?)null);

        // Act
        var act = async () => await _service.ConfirmarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Contribuição não encontrada*");
    }

    [Fact]
    public async Task ConfirmarAsync_DeveLancarExcecao_QuandoContribuicaoJaConfirmada()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado // Já confirmado
        };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);

        // Act
        var act = async () => await _service.ConfirmarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já está confirmada*");
    }

    [Fact]
    public async Task ConfirmarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 999 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 999,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise
        };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Presente?)null);

        // Act
        var act = async () => await _service.ConfirmarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Presente não encontrado*");
    }

    [Fact]
    public async Task ConfirmarAsync_NaoDeveAtualizarPresente_QuandoStatusJaCorreto()
    {
        // Arrange
        var dto = new ConfirmarContribuicaoPresenteDto { IdContribuicao = 1, IdPresente = 10 };
        var contribuicao = new ContribuicaoPresente 
        { 
            Id = 1, 
            IdPresente = 10,
            IdStatusContribuicao = (int)StatusContribuicaoEnum.EmAnalise,
            Valor = 500m
        };
        var presente = new Presente 
        { 
            Id = 10, 
            Valor = 1000m, 
            IdStatus = (int)StatusPresenteEnum.EmArrecadacao // Já está correto
        };

        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contribuicao);
        _presenteRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(presente);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(10)).ReturnsAsync(500m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.ConfirmarAsync(dto);

        // Assert
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once); // Apenas uma vez para a contribuição
    }
}
