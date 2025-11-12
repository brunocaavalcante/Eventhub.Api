using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class ConvidadosRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByEventoAsync_DeveRetornarConvidadosDoEvento()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var eventoId = 1;
       
        context.Fotos.Add(new Fotos { Id = 1, NomeArquivo = "foto1.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" });
        
        context.Convidados.AddRange(
            new Convidados { Id = 1, IdEvento = eventoId, Nome = "Convidado 1", Email = "c1@teste.com", Telefone = "111", IdFoto = 1 },
            new Convidados { Id = 2, IdEvento = 2, Nome = "Convidado 2", Email = "c2@teste.com", Telefone = "222", IdFoto = 1 }
        );
        await context.SaveChangesAsync();
        var repo = new ConvidadosRepository(context);

        // Act
        var convidados = await repo.GetByEventoAsync(eventoId);

        // Assert
        convidados.Should().HaveCount(1);
        convidados.First().IdEvento.Should().Be(eventoId);
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarConvidado_QuandoEmailExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var convidado = new Convidados { Id = 1, IdEvento = 1, Nome = "Convidado 1", Email = "c1@teste.com", Telefone = "111", IdFoto = 1 };
        context.Convidados.Add(convidado);
        await context.SaveChangesAsync();
        var repo = new ConvidadosRepository(context);

        // Act
        var resultado = await repo.GetByEmailAsync("c1@teste.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("c1@teste.com");
    }

    [Fact]
    public async Task GetByEmailAsync_DeveRetornarNull_QuandoEmailNaoExiste()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new ConvidadosRepository(context);

        // Act
        var resultado = await repo.GetByEmailAsync("naoexiste@teste.com");

        // Assert
        resultado.Should().BeNull();
    }
}
