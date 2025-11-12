using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class GaleriaRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByEventoAsync_DeveRetornarGaleriasDoEvento_OrdenadasPorOrdem()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var eventoId = 1;
        // Adiciona fotos relacionadas
        context.Fotos.AddRange(
            new Fotos { Id = 1, NomeArquivo = "foto1.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" },
            new Fotos { Id = 2, NomeArquivo = "foto2.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" },
            new Fotos { Id = 3, NomeArquivo = "foto3.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" }
        );

        context.Galerias.AddRange(
            new Galeria { Id = 1, IdEvento = eventoId, IdFoto = 1, Ordem = 2, Visibilidade = "Público", Legenda = "Foto 2", Data = DateTime.Now },
            new Galeria { Id = 2, IdEvento = eventoId, IdFoto = 2, Ordem = 1, Visibilidade = "Público", Legenda = "Foto 1", Data = DateTime.Now },
            new Galeria { Id = 3, IdEvento = 2, IdFoto = 3, Ordem = 1, Visibilidade = "Privado", Legenda = "Outra", Data = DateTime.Now }
        );
        await context.SaveChangesAsync();
        var repo = new GaleriaRepository(context);

        // Act
        var galerias = (await repo.GetByEventoAsync(eventoId)).ToList();

        // Assert
        galerias.Should().HaveCount(2);
        galerias[0].Ordem.Should().Be(1);
        galerias[1].Ordem.Should().Be(2);
        galerias.Should().OnlyContain(g => g.IdEvento == eventoId);
    }
}
