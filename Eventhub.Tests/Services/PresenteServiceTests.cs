using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class PresenteServiceTests
{
    private readonly Mock<IPresenteRepository> _repoMock;
    private readonly Mock<IStatusPresenteRepository> _statusPresenteRepoMock;
    private readonly Mock<IFotosService> _fotosServiceMock;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PresenteService _service;

    public PresenteServiceTests()
    {
        _repoMock = new Mock<IPresenteRepository>();
        _statusPresenteRepoMock = new Mock<IStatusPresenteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _fotosServiceMock = new Mock<IFotosService>();
        _mapperMock = new Mock<IMapper>();
        _service = new PresenteService(_repoMock.Object, _statusPresenteRepoMock.Object, _fotosServiceMock.Object, _unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarPresente_ComSucesso()
    {
        // Arrange
        var dto = new CreatePresenteDto { Nome = "Notebook", Descricao = "Dell", Valor = 3000 };
        var presente = new Presente { Id = 1, Nome = "Notebook", Descricao = "Dell", Valor = 3000 };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Notebook" };

        _mapperMock.Setup(m => m.Map<Presente>(dto)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Presente>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AdicionarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("Notebook");
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Presente>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new UpdatePresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.AtualizarAsync(dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.RemoverAsync(1);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task ObterPorIdAsync_DeveRetornarDto_QuandoEncontrado()
    {
        // Arrange
        var presente = new Presente { Id = 1, Nome = "Notebook" };
        var dto = new PresenteDto { Id = 1, Nome = "Notebook" };
        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(dto);

        // Act
        var result = await _service.ObterPorIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task ObterDetalhesPorIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdDetalhesAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        var result = await _service.ObterDetalhesPorIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ObterDetalhesPorIdAsync_DeveRetornarDetalhesCompletos_QuandoEncontrado()
    {
        // Arrange
        var presente = new Presente
        {
            Id = 1,
            Nome = "Notebook",
            Valor = 3000,
            Status = new StatusPresente { Id = 1, Descricao = "Disponível" },
            Categoria = new CategoriaPresente { Id = 1, Nome = "Eletrônicos" },
            Contribuicoes = new List<ContribuicaoPresente>
            {
                new ContribuicaoPresente
                {
                    Id = 1,
                    Valor = 500,
                    DataCadastro = DateTime.UtcNow,
                    StatusContribuicao = new StatusContribuicao { Id = 1, Descricao = "Confirmado" },
                    Participante = new Participante
                    {
                        Id = 1,
                        Usuario = new Usuario
                        {
                            Id = 1,
                            Nome = "João Silva",
                            Email = "joao@email.com",
                            Foto = new Fotos
                            {
                                Id = 1,
                                NomeArquivo = "foto.jpg",
                                Url = "https://cdn/foto-usuario.jpg",
                                ContentType = "image/jpeg"
                            }
                        }
                    }
                }
            }
        };

        var detalhesDto = new PresenteDetalhesDto
        {
            Id = 1,
            Nome = "Notebook",
            Valor = 3000,
            Status = new StatusPresenteDto { Id = 1, Descricao = "Disponível" },
            Categoria = new CategoriaPresenteDto { Id = 1, Nome = "Eletrônicos" },
            Contribuicoes = new List<ContribuicaoDetalhesDto>
            {
                new ContribuicaoDetalhesDto
                {
                    Id = 1,
                    Valor = 500,
                    Status = new StatusContribuicaoDto { Id = 1, Descricao = "Confirmado" },
                    Participante = new ParticipanteContribuicaoDto
                    {
                        Id = 1,
                        Nome = "João Silva",
                        Email = "joao@email.com",
                        Foto = "https://cdn/foto-usuario.jpg"
                    }
                }
            }
        };

        _repoMock.Setup(r => r.GetByIdDetalhesAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map<PresenteDetalhesDto>(presente)).Returns(detalhesDto);

        // Act
        var result = await _service.ObterDetalhesPorIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Nome.Should().Be("Notebook");
        result.Valor.Should().Be(3000);
        result.Contribuicoes.Should().HaveCount(1);
        result.Contribuicoes.First().Participante.Nome.Should().Be("João Silva");
    }

    [Fact]
    public async Task ListarTodosAsync_DeveRetornarListaDeDtos()
    {
        // Arrange
        var presentes = new List<Presente> { new Presente { Id = 1, IdEvento = 1, Nome = "Notebook" } };
        var dtos = new List<PresenteDto> { new PresenteDto { Id = 1, IdEvento = 1, Nome = "Notebook" } };
        _repoMock.Setup(r => r.GetByEventIdAsync(1)).ReturnsAsync(presentes);
        _mapperMock.Setup(m => m.Map<IEnumerable<PresenteDto>>(presentes)).Returns(dtos);

        // Act
        var result = await _service.ListarTodosAsync(1);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoPresenteNaoDisponivel()
    {
        // Arrange
        var presente = new Presente { Id = 1, IdStatus = (int)Domain.Enums.StatusPresenteEnum.Reservado };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<System.Linq.Expressions.Expression<Func<Presente, object>>[]>())).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.RemoverAsync(1);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*Somente presentes com status 'Disponível' podem ser removidos*");
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverPresente_ComSucesso()
    {
        // Arrange
        var presente = new Presente { Id = 1, IdStatus = (int)Domain.Enums.StatusPresenteEnum.Disponivel, Galerias = new List<Galeria>() };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<System.Linq.Expressions.Expression<Func<Presente, object>>[]>())).ReturnsAsync(presente);
        _fotosServiceMock.Setup(f => f.RemoverAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.RemoverAsync(1);

        // Assert
        _repoMock.Verify(r => r.Remove(presente), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveLancarExcecao_QuandoErroAoRemoverFoto()
    {
        // Arrange
        var presente = new Presente
        {
            Id = 1,
            IdStatus = (int)Domain.Enums.StatusPresenteEnum.Disponivel,
            Galerias = new List<Galeria>
            {
                new Galeria { IdFoto = 101 }
            }
        };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<System.Linq.Expressions.Expression<Func<Presente, object>>[]>())).ReturnsAsync(presente);
        _fotosServiceMock.Setup(f => f.RemoverAsync(101)).ThrowsAsync(new Exception("Erro ao remover foto"));

        // Act
        Func<Task> act = async () => await _service.RemoverAsync(1);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Erro ao remover foto");
    }

    #region Testes para ProcessarImagensAsync (via AtualizarAsync)

    [Fact]
    public async Task AtualizarAsync_DeveAdicionarNovasImagens_QuandoImagensSemId()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Descricao = "Descricao Teste",
            Valor = 1500,
            Imagens = new List<UpdateFotoDto>
            {
                new UpdateFotoDto { Id = 0, NomeArquivo = "nova-foto.jpg", Base64 = "base64string" }
            }
        };

        var presente = new Presente
        {
            Id = 1,
            Nome = "Presente Antigo",
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Galerias = new List<Galeria>()
        };

        var fotoDto = new FotoDto { Id = 100, NomeArquivo = "nova-foto.jpg" };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.UploadAsync(It.IsAny<UploadFotoDto>())).ReturnsAsync(fotoDto);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.UploadAsync(It.IsAny<UploadFotoDto>()), Times.Once);
        presente.Galerias.Should().HaveCount(1);
        presente.Galerias.First().IdFoto.Should().Be(100);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRemoverImagensExistentes_QuandoNaoEstaoNoDto()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Valor = 1500,
            Descricao = "Descricao Teste",
            Imagens = new List<UpdateFotoDto>() // Nenhuma imagem no DTO
        };

        var galeria1 = new Galeria
        {
            Id = 1,
            IdFoto = 50,            
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var galeria2 = new Galeria
        {
            Id = 2,
            IdFoto = 51,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var presente = new Presente
        {
            Id = 1,
            Nome = "Presente Antigo",
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Galerias = new List<Galeria> { galeria1, galeria2 }
        };

        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.RemoverAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.RemoverAsync(50), Times.Once);
        _fotosServiceMock.Verify(f => f.RemoverAsync(51), Times.Once);
        presente.Galerias.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarImagensExistentes_QuandoImagensComId()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Valor = 1500,
            Descricao = "Descricao Teste",
            Imagens = new List<UpdateFotoDto>
            {
                new UpdateFotoDto { Id = 50, NomeArquivo = "foto-atualizada.jpg" }
            }
        };

        var galeria = new Galeria
        {
            Id = 1,
            IdFoto = 50,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var presente = new Presente
        {
            Id = 1,
            Nome = "Presente Antigo",
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Galerias = new List<Galeria> { galeria }
        };

        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.UpdateAsync(It.IsAny<UpdateFotoDto>())).ReturnsAsync(new FotoDto());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.UpdateAsync(It.Is<UpdateFotoDto>(x => x.Id == 50)), Times.Once);
        _fotosServiceMock.Verify(f => f.RemoverAsync(It.IsAny<int>()), Times.Never);
        presente.Galerias.Should().HaveCount(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveTratarCenarioMisto_AdicionarAtualizarERemoverImagens()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Valor = 1500,
            Descricao = "Descricao Teste",
            Imagens = new List<UpdateFotoDto>
            {
                new UpdateFotoDto { Id = 50, NomeArquivo = "foto-atualizada.jpg" }, // Atualizar
                new UpdateFotoDto { Id = 0, NomeArquivo = "nova-foto.jpg", Base64 = "base64string" } // Adicionar
                // Foto com Id 51 será removida (não está no DTO)
            }
        };

        var galeria1 = new Galeria
        {
            Id = 1,
            IdFoto = 50,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var galeria2 = new Galeria
        {
            Id = 2,
            IdFoto = 51,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var presente = new Presente
        {
            Id = 1,
            IdEvento = 10,
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Nome = "Presente Antigo",
            Galerias = new List<Galeria> { galeria1, galeria2 }
        };

        var fotoDto = new FotoDto { Id = 100, NomeArquivo = "nova-foto.jpg" };
        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.UploadAsync(It.IsAny<UploadFotoDto>())).ReturnsAsync(fotoDto);
        _fotosServiceMock.Setup(f => f.RemoverAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.UpdateAsync(It.Is<UpdateFotoDto>(x => x.Id == 50)), Times.Once);
        _fotosServiceMock.Verify(f => f.RemoverAsync(51), Times.Once);
        _fotosServiceMock.Verify(f => f.UploadAsync(It.IsAny<UploadFotoDto>()), Times.Once);
        presente.Galerias.Should().HaveCount(2); // 1 atualizada + 1 nova (a removida não conta)
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_NaoDeveRemoverGaleriasDeOutrosTipos()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Valor = 1500,
            Descricao = "Descricao Teste",
            Imagens = new List<UpdateFotoDto>() // Nenhuma imagem de produto
        };

        var galeriaProduto = new Galeria
        {
            Id = 1,
            IdFoto = 50,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var galeriaEvento = new Galeria
        {
            Id = 2,
            IdFoto = 60,
            Tipo = Domain.Enums.GaleriaTipo.Local
        };

        var presente = new Presente
        {
            Id = 1,
            IdEvento = 10,
            Nome = "Presente Antigo",
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Galerias = new List<Galeria> { galeriaProduto, galeriaEvento }
        };

        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.RemoverAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.RemoverAsync(50), Times.Once); // Remove apenas Produto
        _fotosServiceMock.Verify(f => f.RemoverAsync(60), Times.Never); // Não remove Evento
        presente.Galerias.Should().HaveCount(1); // Apenas galeriaEvento permanece
        presente.Galerias.First().Tipo.Should().Be(Domain.Enums.GaleriaTipo.Local);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveProcessarCorretamente_QuandoImagensNull()
    {
        // Arrange
        var dto = new UpdatePresenteDto
        {
            Id = 1,
            Nome = "Presente Teste",
            Valor = 1500,
            Descricao = "Descricao Teste",
            Imagens = null // Imagens é null
        };

        var galeria = new Galeria
        {
            Id = 1,
            IdFoto = 50,
            Tipo = Domain.Enums.GaleriaTipo.Produto
        };

        var presente = new Presente
        {
            Id = 1,
            Nome = "Presente Antigo",
            Valor = 1200,
            Descricao = "Descricao Antiga",
            Galerias = new List<Galeria> { galeria }
        };

        var presenteDto = new PresenteDto { Id = 1, Nome = "Presente Teste" };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _mapperMock.Setup(m => m.Map(dto, presente)).Returns(presente);
        _mapperMock.Setup(m => m.Map<PresenteDto>(presente)).Returns(presenteDto);
        _fotosServiceMock.Setup(f => f.RemoverAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AtualizarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        _fotosServiceMock.Verify(f => f.RemoverAsync(50), Times.Once);
        presente.Galerias.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region ReservarPresente

    [Fact]
    public async Task ReservarPresenteAsync_DeveReservar_ComSucesso()
    {
        // Arrange
        var dto = new ReservarPresenteDto { IdParticipante = 123 };
        var presente = new Presente 
        { 
            Id = 1, 
            Nome = "Notebook", 
            IdStatus = 1, // Disponível
            Contribuicoes = new List<ContribuicaoPresente>()
        };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.ReservarPresenteAsync(1, dto);

        // Assert
        presente.IdStatus.Should().Be(2); // Reservado
        presente.IdParticipanteReservou.Should().Be(123);
        presente.DataReserva.Should().NotBeNull();
        _repoMock.Verify(r => r.Update(It.IsAny<Presente>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ReservarPresenteAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new ReservarPresenteDto { IdParticipante = 123 };
        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.ReservarPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task ReservarPresenteAsync_DeveLancarExcecao_QuandoPresenteNaoEstaDisponivel()
    {
        // Arrange
        var dto = new ReservarPresenteDto { IdParticipante = 123 };
        var presente = new Presente 
        { 
            Id = 1, 
            IdStatus = 2 // Já reservado
        };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.ReservarPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não está disponível*");
    }

    [Fact]
    public async Task ReservarPresenteAsync_DeveLancarExcecao_QuandoPresentePossuiContribuicoes()
    {
        // Arrange
        var dto = new ReservarPresenteDto { IdParticipante = 123 };
        var presente = new Presente 
        { 
            Id = 1, 
            IdStatus = 1,
            Contribuicoes = new List<ContribuicaoPresente> 
            { 
                new ContribuicaoPresente { Id = 1, Valor = 100 } 
            }
        };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.ReservarPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*possui contribuições*");
    }

    [Fact]
    public async Task ReservarPresenteAsync_DeveLancarExcecao_QuandoPresenteJaReservado()
    {
        // Arrange
        var dto = new ReservarPresenteDto { IdParticipante = 123 };
        var presente = new Presente 
        { 
            Id = 1, 
            IdStatus = 1,
            Contribuicoes = new List<ContribuicaoPresente>(),
            IdParticipanteReservou = 999 // Já reservado por outro
        };

        _repoMock.Setup(r => r.GetByIdCompletoAsync(1)).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.ReservarPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já está reservado*");
    }

    #endregion

    #region CancelarReservaPresente

    [Fact]
    public async Task CancelarReservaPresenteAsync_DeveCancelar_ComSucesso()
    {
        // Arrange
        var dto = new CancelarReservaPresenteDto 
        { 
            IdParticipante = 123,
            Justificativa = "Consegui melhor preço em outro lugar"
        };
        var presente = new Presente 
        { 
            Id = 1, 
            Nome = "Notebook", 
            IdStatus = 2, // Reservado
            IdParticipanteReservou = 123,
            DataReserva = DateTime.UtcNow.AddDays(-1)
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(presente);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _service.CancelarReservaPresenteAsync(1, dto);

        // Assert
        presente.IdStatus.Should().Be(1); // Disponível
        presente.IdParticipanteReservou.Should().BeNull();
        presente.DataReserva.Should().BeNull();
        _repoMock.Verify(r => r.Update(It.IsAny<Presente>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelarReservaPresenteAsync_DeveLancarExcecao_QuandoPresenteNaoEncontrado()
    {
        // Arrange
        var dto = new CancelarReservaPresenteDto 
        { 
            IdParticipante = 123,
            Justificativa = "Motivo válido"
        };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Presente?)null);

        // Act
        Func<Task> act = async () => await _service.CancelarReservaPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task CancelarReservaPresenteAsync_DeveLancarExcecao_QuandoPresenteNaoEstaReservado()
    {
        // Arrange
        var dto = new CancelarReservaPresenteDto 
        { 
            IdParticipante = 123,
            Justificativa = "Motivo válido"
        };
        var presente = new Presente 
        { 
            Id = 1, 
            IdStatus = 1 // Disponível (não reservado)
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.CancelarReservaPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não está reservado*");
    }

    [Fact]
    public async Task CancelarReservaPresenteAsync_DeveLancarExcecao_QuandoParticipanteDiferente()
    {
        // Arrange
        var dto = new CancelarReservaPresenteDto 
        { 
            IdParticipante = 123,
            Justificativa = "Motivo válido"
        };
        var presente = new Presente 
        { 
            Id = 1, 
            IdStatus = 2,
            IdParticipanteReservou = 999 // Reservado por outro participante
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(presente);

        // Act
        Func<Task> act = async () => await _service.CancelarReservaPresenteAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não tem permissão*");
    }

    #endregion

    #region ObterStatusPresentesAsync

    [Fact]
    public async Task ObterStatusPresentesAsync_DeveRetornarListaDeStatusPresente_ComSucesso()
    {
        // Arrange
        var statusPresentes = new List<StatusPresente>
        {
            new StatusPresente { Id = 1, Descricao = "Disponível" },
            new StatusPresente { Id = 2, Descricao = "Reservado" },
            new StatusPresente { Id = 3, Descricao = "Em Arrecadação" }
        };
        var statusPresenteDtos = new List<StatusPresenteDto>
        {
            new StatusPresenteDto { Id = 1, Descricao = "Disponível" },
            new StatusPresenteDto { Id = 2, Descricao = "Reservado" },
            new StatusPresenteDto { Id = 3, Descricao = "Em Arrecadação" }
        };

        _statusPresenteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(statusPresentes);
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusPresenteDto>>(statusPresentes)).Returns(statusPresenteDtos);

        // Act
        var result = (await _service.ObterStatusPresentesAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Descricao == "Disponível");
        result.Should().Contain(s => s.Descricao == "Reservado");
        _statusPresenteRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterStatusPresentesAsync_DeveRetornarListaVazia_QuandoNaoHouverRegistros()
    {
        // Arrange
        _statusPresenteRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<StatusPresente>());
        _mapperMock.Setup(m => m.Map<IEnumerable<StatusPresenteDto>>(It.IsAny<IEnumerable<StatusPresente>>()))
            .Returns(new List<StatusPresenteDto>());

        // Act
        var result = await _service.ObterStatusPresentesAsync();

        // Assert
        result.Should().BeEmpty();
        _statusPresenteRepoMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion
}