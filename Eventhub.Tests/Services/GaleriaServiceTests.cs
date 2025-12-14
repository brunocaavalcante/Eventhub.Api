using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class GaleriaServiceTests
{
    private readonly Mock<IGaleriaRepository> _galeriaRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly GaleriaService _galeriaService;

    public GaleriaServiceTests()
    {
        _galeriaRepositoryMock = new Mock<IGaleriaRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _galeriaService = new GaleriaService(_galeriaRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarGaleria_QuandoDadosValidos()
    {
        // Arrange
        var galeria = new Galeria
        {
            Id = 1,
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 1,
            Visibilidade = "Público",
            Legenda = "Foto do evento",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Galeria
        };

        _galeriaRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Galeria>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _galeriaService.AdicionarAsync(galeria);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Legenda.Should().Be("Foto do evento");
        resultado.Ordem.Should().Be(1);
        _galeriaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Galeria>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoVisibilidadeVazia()
    {
        // Arrange
        var galeria = new Galeria
        {
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 1,
            Visibilidade = "",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Galeria
        };

        // Act
        Func<Task> act = async () => await _galeriaService.AdicionarAsync(galeria);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*visibilidade*");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoOrdemNegativa()
    {
        // Arrange
        var galeria = new Galeria
        {
            IdEvento = 1,
            IdFoto = 1,
            Ordem = -1,
            Visibilidade = "Público",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Galeria
        };

        // Act
        Func<Task> act = async () => await _galeriaService.AdicionarAsync(galeria);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*ordem*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarGaleria_QuandoDadosValidos()
    {
        // Arrange
        var galeria = new Galeria
        {
            Id = 1,
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 2,
            Visibilidade = "Privado",
            Legenda = "Legenda atualizada",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Galeria
        };

        _galeriaRepositoryMock.Setup(x => x.GetByIdAsync(galeria.Id))
            .ReturnsAsync(galeria);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _galeriaService.AtualizarAsync(galeria);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Legenda.Should().Be("Legenda atualizada");
        resultado.Visibilidade.Should().Be("Privado");
        _galeriaRepositoryMock.Verify(x => x.GetByIdAsync(galeria.Id), Times.Once);
        _galeriaRepositoryMock.Verify(x => x.Update(It.IsAny<Galeria>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverGaleria_QuandoGaleriaExiste()
    {
        // Arrange
        var galeriaId = 1;
        var galeria = new Galeria
        {
            Id = galeriaId,
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 1,
            Visibilidade = "Público",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Galeria
        };

        _galeriaRepositoryMock.Setup(x => x.GetByIdAsync(galeriaId))
            .ReturnsAsync(galeria);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _galeriaService.RemoverAsync(galeriaId);

        // Assert
        _galeriaRepositoryMock.Verify(x => x.GetByIdAsync(galeriaId), Times.Once);
        _galeriaRepositoryMock.Verify(x => x.Remove(It.IsAny<Galeria>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorEventoAsync_DeveRetornarGalerias_QuandoEventoExiste()
    {
        // Arrange
        var eventoId = 1;
        var galerias = new List<Galeria>
        {
            new Galeria { Id = 1, IdEvento = eventoId, IdFoto = 1, Ordem = 1, Visibilidade = "Público", Legenda = "Foto 1", Data = DateTime.Now, Tipo = GaleriaTipo.Galeria },
            new Galeria { Id = 2, IdEvento = eventoId, IdFoto = 2, Ordem = 2, Visibilidade = "Público", Legenda = "Foto 2", Data = DateTime.Now, Tipo = GaleriaTipo.Galeria }
        };

        _galeriaRepositoryMock.Setup(x => x.GetByEventoAsync(eventoId))
            .ReturnsAsync(galerias);

        // Act
        var resultado = await _galeriaService.ObterPorEventoAsync(eventoId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().OnlyContain(g => g.IdEvento == eventoId);
        resultado.Should().BeInAscendingOrder(g => g.Ordem);
        _galeriaRepositoryMock.Verify(x => x.GetByEventoAsync(eventoId), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoEventoJaPossuiFotoCapa()
    {
        var galeria = new Galeria
        {
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 1,
            Visibilidade = "Público",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Capa
        };

        _galeriaRepositoryMock.Setup(x => x.ExistsByTipoAsync(galeria.IdEvento, GaleriaTipo.Capa, null))
            .ReturnsAsync(true);

        Func<Task> act = () => _galeriaService.AdicionarAsync(galeria); 

        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*capa*");
    }

    [Fact]
    public async Task AtualizarAsync_DevePermitirCapa_QuandoNaoExisteOutra()
    {
        var galeria = new Galeria
        {
            Id = 1,
            IdEvento = 1,
            IdFoto = 1,
            Ordem = 1,
            Visibilidade = "Público",
            Data = DateTime.Now,
            Tipo = GaleriaTipo.Capa
        };

        _galeriaRepositoryMock.Setup(x => x.GetByIdAsync(galeria.Id)).ReturnsAsync(galeria);
        _galeriaRepositoryMock.Setup(x => x.ExistsByTipoAsync(galeria.IdEvento, GaleriaTipo.Capa, galeria.Id))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

        var resultado = await _galeriaService.AtualizarAsync(galeria);

        resultado.Tipo.Should().Be(GaleriaTipo.Capa);
        _galeriaRepositoryMock.Verify(x => x.ExistsByTipoAsync(galeria.IdEvento, GaleriaTipo.Capa, galeria.Id), Times.Once);
    }
}
