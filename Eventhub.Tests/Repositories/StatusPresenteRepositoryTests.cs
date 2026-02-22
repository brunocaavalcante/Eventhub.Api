using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class StatusPresenteRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosOsStatusPresente()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Set<StatusPresente>().AddRange(
            new StatusPresente { Id = 1, Descricao = "Disponível" },
            new StatusPresente { Id = 2, Descricao = "Reservado" },
            new StatusPresente { Id = 3, Descricao = "Em Arrecadação" }
        );
        await context.SaveChangesAsync();
        var repo = new StatusPresenteRepository(context);

        // Act
        var result = (await repo.GetAllAsync()).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Descricao == "Disponível");
        result.Should().Contain(s => s.Descricao == "Reservado");
        result.Should().Contain(s => s.Descricao == "Em Arrecadação");
    }

    [Fact]
    public async Task GetAllAsync_SemRegistros_DeveRetornarListaVazia()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new StatusPresenteRepository(context);

        // Act
        var result = await repo.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarStatusPresente_QuandoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Set<StatusPresente>().Add(new StatusPresente { Id = 1, Descricao = "Disponível" });
        await context.SaveChangesAsync();
        var repo = new StatusPresenteRepository(context);

        // Act
        var result = await repo.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Descricao.Should().Be("Disponível");
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoNaoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new StatusPresenteRepository(context);

        // Act
        var result = await repo.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
