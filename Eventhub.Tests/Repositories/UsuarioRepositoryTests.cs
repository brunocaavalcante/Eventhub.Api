using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class UsuarioRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarUsuario_QuandoEmailExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var usuario = new Usuario { Id = 1, Nome = "João", Email = "joao@teste.com", Status = "Ativo", DataCadastro = DateTime.Now };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        var repo = new UsuarioRepository(context);

        // Act
        var resultado = await repo.GetByEmailAsync("joao@teste.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("joao@teste.com");
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarNull_QuandoEmailNaoExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new UsuarioRepository(context);

        // Act
        var resultado = await repo.GetByEmailAsync("naoexiste@teste.com");

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_DeveRetornarTrue_QuandoEmailExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var usuario = new Usuario { Id = 1, Nome = "Maria", Email = "maria@teste.com", Status = "Ativo", DataCadastro = DateTime.Now };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        var repo = new UsuarioRepository(context);

        // Act
        var existe = await repo.EmailExistsAsync("maria@teste.com");

        // Assert
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_DeveRetornarFalse_QuandoEmailNaoExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new UsuarioRepository(context);

        // Act
        var existe = await repo.EmailExistsAsync("naoexiste@teste.com");

        // Assert
        existe.Should().BeFalse();
    }
}
