using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class PresenteRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPresente_QuandoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Presentes.Add(new Presente 
        { 
            Id = 1, 
            Nome = "Notebook", 
            Descricao = "Notebook Dell",
            Valor = 3000
        });
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presente = await repo.GetByIdAsync(1);

        // Assert
        presente.Should().NotBeNull();
        presente!.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosOsPresentes()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Presentes.AddRange(
            new Presente { Id = 1, Nome = "Presente 1", Valor = 100, Descricao = "Desc 1" },
            new Presente { Id = 2, Nome = "Presente 2", Valor = 200, Descricao = "Desc 2" }
        );
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presentes = (await repo.GetAllAsync()).ToList();

        // Assert
        presentes.Should().HaveCount(2);
    }
}