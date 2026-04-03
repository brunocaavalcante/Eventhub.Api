using AutoMapper;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Interfaces;
using Eventhub.Tests.Helpers;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Eventhub.Tests.Services;

public class PerfilServiceTests
{
    private readonly Mock<IPerfilRepository> _perfilRepositoryMock;
    private readonly Mock<IModuloRepository> _moduloRepositoryMock;
    private readonly Mock<IPerfilEventoPermissaoRepository> _perfilEventoPermissaoRepositoryMock;
    private readonly IMapper _mapper;
    private readonly PerfilService _perfilService;

    public PerfilServiceTests()
    {
        _perfilRepositoryMock = new Mock<IPerfilRepository>();
        _moduloRepositoryMock = new Mock<IModuloRepository>();
        _perfilEventoPermissaoRepositoryMock = new Mock<IPerfilEventoPermissaoRepository>();
        _mapper = AutoMapperHelper.CreateMapper();

        _perfilService = new PerfilService(
            _perfilRepositoryMock.Object,
            _moduloRepositoryMock.Object,
            _perfilEventoPermissaoRepositoryMock.Object,
            _mapper);
    }

    #region ObterPerfisAtivosAsync

    [Fact]
    public async Task ObterPerfisAtivosAsync_DeveRetornarListaDePerfis_QuandoExistemPerfisAtivos()
    {
        // Arrange
        var perfisAtivos = new List<Perfil>
        {
            new Perfil { Id = 1, Descricao = "Administrador", Icon = "admin-icon", Status = 'A' },
            new Perfil { Id = 2, Descricao = "Músico", Icon = "music-icon", Status = 'A' },
            new Perfil { Id = 3, Descricao = "Convidado", Icon = "guest-icon", Status = 'A' }
        };

        _perfilRepositoryMock.Setup(x => x.GetPerfisAtivosAsync())
            .ReturnsAsync(perfisAtivos);

        // Act
        var resultado = await _perfilService.ObterPerfisAtivosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(3);
        resultado.First().Descricao.Should().Be("Administrador");
        _perfilRepositoryMock.Verify(x => x.GetPerfisAtivosAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPerfisAtivosAsync_DeveRetornarListaVazia_QuandoNaoExistemPerfisAtivos()
    {
        // Arrange
        var perfisAtivos = new List<Perfil>();

        _perfilRepositoryMock.Setup(x => x.GetPerfisAtivosAsync())
            .ReturnsAsync(perfisAtivos);

        // Act
        var resultado = await _perfilService.ObterPerfisAtivosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
        _perfilRepositoryMock.Verify(x => x.GetPerfisAtivosAsync(), Times.Once);
    }

    #endregion

    #region ObterModulosPerfilAsync

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveLancarExcecao_QuandoIdEventoEhZero()
    {
        // Arrange
        var idPerfil = 1;
        var idEvento = 0;

        // Act
        Func<Task> act = async () => await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("O id do evento deve ser informado para obter as permissões do perfil.");
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveUsarPerfilConvidado_QuandoIdPerfilEhZero()
    {
        // Arrange
        var idPerfil = 0;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 }
        };

        var permissoes = new List<PerfilEventoPermissao>
        {
            new PerfilEventoPermissao
            {
                Id = 1,
                IdPerfil = (int)EnumPerfil.Convidado,
                IdEvento = idEvento,
                IdPermissao = 1,
                Concedida = true,
                Perfil = new Perfil { Id = (int)EnumPerfil.Convidado, Status = 'A' },
                Permissao = new Permissao { Id = 1, IdModulo = 1 }
            }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos);

        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync((int)EnumPerfil.Convidado, idEvento))
            .ReturnsAsync(permissoes);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        _perfilEventoPermissaoRepositoryMock.Verify(x => x.GetByPerfilEventoAsync((int)EnumPerfil.Convidado, idEvento), Times.Once);
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveRetornarTodosModulos_QuandoPerfilEhAdministrador()
    {
        // Arrange
        var idPerfil = (int)EnumPerfil.Administrador;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 },
            new Modulo { Id = 2, Nome = "Eventos", ShowInMenu = true, Ordem = 2 },
            new Modulo { Id = 3, Nome = "Usuários", ShowInMenu = true, Ordem = 3 }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(3);
        resultado.Should().BeInAscendingOrder(m => m.Ordem);
        _moduloRepositoryMock.Verify(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()), Times.Once);
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveRetornarModulosComPermissoes_QuandoPerfilNaoEhAdministrador()
    {
        // Arrange
        var idPerfil = (int)EnumPerfil.Musico;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 },
            new Modulo { Id = 2, Nome = "Programação", ShowInMenu = true, Ordem = 2 },
            new Modulo { Id = 3, Nome = "Configurações", ShowInMenu = true, Ordem = 3 }
        };

        var permissoes = new List<PerfilEventoPermissao>
        {
            new PerfilEventoPermissao
            {
                Id = 1,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 1,
                Concedida = true,
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 1, IdModulo = 1 }
            },
            new PerfilEventoPermissao
            {
                Id = 2,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 2,
                Concedida = true,
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 2, IdModulo = 2 }
            },
            new PerfilEventoPermissao
            {
                Id = 3,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 3,
                Concedida = false, // Permissão não concedida
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 3, IdModulo = 3 }
            }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos);

        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync(idPerfil, idEvento))
            .ReturnsAsync(permissoes);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2); // Apenas módulos com permissões concedidas
        resultado.Should().Contain(m => m.Id == 1);
        resultado.Should().Contain(m => m.Id == 2);
        resultado.Should().NotContain(m => m.Id == 3);
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveUsarPermissoesConvidado_QuandoNaoExistemPermissoesParaPerfil()
    {
        // Arrange
        var idPerfil = (int)EnumPerfil.Musico;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 }
        };

        var permissoesConvidado = new List<PerfilEventoPermissao>
        {
            new PerfilEventoPermissao
            {
                Id = 1,
                IdPerfil = (int)EnumPerfil.Convidado,
                IdEvento = idEvento,
                IdPermissao = 1,
                Concedida = true,
                Perfil = new Perfil { Id = (int)EnumPerfil.Convidado, Status = 'A' },
                Permissao = new Permissao { Id = 1, IdModulo = 1 }
            }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos);

        // Primeira chamada retorna lista vazia (sem permissões para o perfil)
        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync(idPerfil, idEvento))
            .ReturnsAsync(new List<PerfilEventoPermissao>());

        // Segunda chamada retorna permissões do convidado
        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync((int)EnumPerfil.Convidado, idEvento))
            .ReturnsAsync(permissoesConvidado);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        _perfilEventoPermissaoRepositoryMock.Verify(x => x.GetByPerfilEventoAsync(idPerfil, idEvento), Times.Once);
        _perfilEventoPermissaoRepositoryMock.Verify(x => x.GetByPerfilEventoAsync((int)EnumPerfil.Convidado, idEvento), Times.Once);
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveRetornarListaVazia_QuandoNaoExistemPermissoesConcedidas()
    {
        // Arrange
        var idPerfil = (int)EnumPerfil.Musico;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 }
        };

        var permissoes = new List<PerfilEventoPermissao>
        {
            new PerfilEventoPermissao
            {
                Id = 1,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 1,
                Concedida = false, // Permissão não concedida
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 1, IdModulo = 1 }
            }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos);

        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync(idPerfil, idEvento))
            .ReturnsAsync(permissoes);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterModulosPerfilAsync_DeveFiltrarModulosQueNaoEstaoNoMenu()
    {
        // Arrange
        var idPerfil = (int)EnumPerfil.Musico;
        var idEvento = 1;

        var modulos = new List<Modulo>
        {
            new Modulo { Id = 1, Nome = "Dashboard", ShowInMenu = true, Ordem = 1 },
            new Modulo { Id = 2, Nome = "Módulo Oculto", ShowInMenu = false, Ordem = 2 }
        };

        var permissoes = new List<PerfilEventoPermissao>
        {
            new PerfilEventoPermissao
            {
                Id = 1,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 1,
                Concedida = true,
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 1, IdModulo = 1 }
            },
            new PerfilEventoPermissao
            {
                Id = 2,
                IdPerfil = idPerfil,
                IdEvento = idEvento,
                IdPermissao = 2,
                Concedida = true,
                Perfil = new Perfil { Id = idPerfil, Status = 'A' },
                Permissao = new Permissao { Id = 2, IdModulo = 2 }
            }
        };

        _moduloRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Modulo, bool>>>()))
            .ReturnsAsync(modulos.Where(m => m.ShowInMenu).ToList());

        _perfilEventoPermissaoRepositoryMock.Setup(x => x.GetByPerfilEventoAsync(idPerfil, idEvento))
            .ReturnsAsync(permissoes);

        // Act
        var resultado = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(1);
        resultado.First().Nome.Should().Be("Dashboard");
    }

    #endregion
}