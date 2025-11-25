using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Eventhub.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly UsuarioService _usuarioService;

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _usuarioService = new UsuarioService(
            _usuarioRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _keycloakServiceMock.Object);
    }

    [Fact]
    public async Task AdicionarAsync_DeveAdicionarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 1,
            Nome = "João Silva",
            Email = "joao@teste.com",
            Telefone = "11999999999",
            Status = "Ativo",
            DataCadastro = DateTime.Now
        };

        _usuarioRepositoryMock.Setup(x => x.EmailExistsAsync(usuario.Email))
            .ReturnsAsync(false);
        _usuarioRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Usuario>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _usuarioService.AdicionarAsync(usuario);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("João Silva");
        resultado.Email.Should().Be("joao@teste.com");
        _usuarioRepositoryMock.Verify(x => x.EmailExistsAsync(usuario.Email), Times.Once);
        _usuarioRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Usuario>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoEmailJaExiste()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nome = "João Silva",
            Email = "joao@teste.com",
            Status = "Ativo"
        };

        _usuarioRepositoryMock.Setup(x => x.EmailExistsAsync(usuario.Email))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _usuarioService.AdicionarAsync(usuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*já cadastrado*");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoNomeVazio()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nome = "",
            Email = "joao@teste.com",
            Status = "Ativo"
        };

        // Act
        Func<Task> act = async () => await _usuarioService.AdicionarAsync(usuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*nome*");
    }

    [Fact]
    public async Task AdicionarAsync_DeveLancarExcecao_QuandoEmailInvalido()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nome = "João Silva",
            Email = "emailinvalido",
            Status = "Ativo"
        };

        // Act
        Func<Task> act = async () => await _usuarioService.AdicionarAsync(usuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*inválido*");
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarUsuario_QuandoDadosValidos()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 1,
            Nome = "João Silva Atualizado",
            Email = "joao@teste.com",
            Status = "Ativo",
            DataCadastro = DateTime.Now
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuario.Id))
            .ReturnsAsync(usuario);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _usuarioService.AtualizarAsync(usuario);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("João Silva Atualizado");
        _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(usuario.Id), Times.Once);
        _usuarioRepositoryMock.Verify(x => x.Update(It.IsAny<Usuario>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = 999,
            Nome = "João Silva",
            Email = "joao@teste.com",
            Status = "Ativo"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuario.Id))
            .ReturnsAsync((Usuario?)null);

        // Act
        Func<Task> act = async () => await _usuarioService.AtualizarAsync(usuario);

        // Assert
        await act.Should().ThrowAsync<ExceptionValidation>()
            .WithMessage("*não encontrado*");
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverUsuario_QuandoUsuarioExiste()
    {
        // Arrange
        var usuarioId = 1;
        var usuario = new Usuario
        {
            Id = usuarioId,
            Nome = "João Silva",
            Email = "joao@teste.com",
            Status = "Ativo"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByIdAsync(usuarioId))
            .ReturnsAsync(usuario);
        _unitOfWorkMock.Setup(x => x.CommitTransactionAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _usuarioService.RemoverAsync(usuarioId);

        // Assert
        _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(usuarioId), Times.Once);
        _usuarioRepositoryMock.Verify(x => x.Remove(It.IsAny<Usuario>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterPorEmailAsync_DeveRetornarUsuario_QuandoEmailExiste()
    {
        // Arrange
        var email = "joao@teste.com";
        var usuario = new Usuario
        {
            Id = 1,
            Nome = "João Silva",
            Email = email,
            Status = "Ativo"
        };

        _usuarioRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(usuario);

        // Act
        var resultado = await _usuarioService.ObterPorEmailAsync(email);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be(email);
        _usuarioRepositoryMock.Verify(x => x.GetByEmailAsync(email), Times.Once);
    }
}
