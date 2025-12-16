using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class TipoEventoRepositoryTests
{
    private readonly EventhubDbContext _context;
    private readonly TipoEventoRepository _repository;

    public TipoEventoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(databaseName: "TipoEventoRepositoryTestsDb")
            .Options;
        _context = new EventhubDbContext(options);
        _repository = new TipoEventoRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosOsTiposEvento()
    {
        // Arrange
        _context.TipoEvento.Add(new TipoEvento { Id = 1, Nome = "Teste", Icon = "icon", Descricao = "desc", IdFoto = 2 });
        _context.TipoEvento.Add(new TipoEvento { Id = 2, Nome = "Outro", Icon = "icon2", Descricao = "desc2", IdFoto = 3 });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_SemRegistros_DeveRetornarListaVazia()
    {
        // Act
        _context.TipoEvento.RemoveRange(_context.TipoEvento);
        await _context.SaveChangesAsync();
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
