using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class PerfilRepositoryTests
{
    private EventhubDbContext GetDbContextWithPerfis()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(databaseName: "PerfisTestDb")
            .Options;
        var context = new EventhubDbContext(options);
        context.Perfis.AddRange(new List<Perfil>
            {
                new Perfil { Id = 1, Descricao = "Administrador", Icon = "admin_panel_settings", Status = 'A' },
                new Perfil { Id = 2, Descricao = "Músico", Icon = "music_note", Status = 'A' },
                new Perfil { Id = 3, Descricao = "Inativo", Icon = "block", Status = 'I' }
            });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetPerfisAtivosAsync_ReturnsOnlyActivePerfis()
    {
        // Arrange
        var context = GetDbContextWithPerfis();
        var repo = new PerfilRepository(context);

        // Act
        var result = await repo.GetPerfisAtivosAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal('A', p.Status));
    }
}