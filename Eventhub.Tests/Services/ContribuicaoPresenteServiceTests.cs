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
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ContribuicaoPresenteService _service;

    public ContribuicaoPresenteServiceTests()
    {
        _contribuicaoRepoMock = new Mock<IContribuicaoPresenteRepository>();
        _presenteRepoMock = new Mock<IPresenteRepository>();
        _fotosServiceMock = new Mock<IFotosService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new ContribuicaoPresenteService(
            _contribuicaoRepoMock.Object,
            _presenteRepoMock.Object,
            _fotosServiceMock.Object,
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

    [Fact]
    public async Task CriarAsync_DeveAtualizarPresenteParaReservado_QuandoTotalAtingeValor()
    {
        var dto = CriarDtoValido();
        var presente = new Presente { Id = dto.IdPresente, Valor = 100m, IdStatus = (int)StatusPresenteEnum.Disponivel };
        var fotoDto = new FotoDto { Id = 10 };
        var contribuicao = new ContribuicaoPresente();

        _presenteRepoMock.Setup(r => r.GetByIdAsync(dto.IdPresente)).ReturnsAsync(presente);
        _fotosServiceMock.Setup(f => f.UploadAsync(dto.Comprovante)).ReturnsAsync(fotoDto);
        _mapperMock.Setup(m => m.Map<ContribuicaoPresente>(dto)).Returns(contribuicao);
        _contribuicaoRepoMock.Setup(r => r.AddAsync(contribuicao)).Returns(Task.CompletedTask);
        _contribuicaoRepoMock.Setup(r => r.GetTotalContribuidoAsync(dto.IdPresente)).ReturnsAsync(100m);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.CriarAsync(dto);

        presente.IdStatus.Should().Be((int)StatusPresenteEnum.Reservado);
        _presenteRepoMock.Verify(r => r.Update(presente), Times.Once);
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
    public async Task CancelarAsync_DeveCancelarComJustificativa()
    {
        // Arrange
        var dto = new CancelarContribuicaoPresenteDto { IdContribuicao = 1, Justificativa = "Duplicidade" };
        var entity = new ContribuicaoPresente { Id = 1, IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado };
        _contribuicaoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        // Act
        await _service.CancelarAsync(dto);

        // Assert
        entity.IdStatusContribuicao.Should().Be((int)StatusContribuicaoEnum.Cancelado);
        entity.Justificativa.Should().Be("Duplicidade");
        _contribuicaoRepoMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
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
}
