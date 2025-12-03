using Eventhub.Domain.Entities;
using Eventhub.Infra.Repositories;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories
{
    public class FotosRepositoryTests
    {
        private readonly EventhubDbContext _context;
        private readonly FotosRepository _repository;

        public FotosRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<EventhubDbContext>()
                .UseInMemoryDatabase(databaseName: "FotosRepositoryTests")
                .Options;
            _context = new EventhubDbContext(options);
            _repository = new FotosRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddFoto()
        {
            var foto = new Fotos { NomeArquivo = "img.jpg", Base64 = "base64" };
            await _repository.AddAsync(foto);
            await _context.SaveChangesAsync();
            var result = await _repository.GetByIdAsync(foto.Id);
            Assert.NotNull(result);
            Assert.Equal("img.jpg", result.NomeArquivo);
        }

        [Fact]
        public async Task Update_ShouldUpdateFoto()
        {
            var foto = new Fotos { NomeArquivo = "img.jpg", Base64 = "base64" };
            await _repository.AddAsync(foto);
            await _context.SaveChangesAsync();
            foto.NomeArquivo = "img2.jpg";
            _repository.Update(foto);
            await _context.SaveChangesAsync();
            var result = await _repository.GetByIdAsync(foto.Id);
            Assert.NotNull(result);
            Assert.Equal("img2.jpg", result.NomeArquivo);
        }

        [Fact]
        public async Task Remove_ShouldDeleteFoto()
        {
            var foto = new Fotos { NomeArquivo = "img.jpg", Base64 = "base64" };
            await _repository.AddAsync(foto);
            await _context.SaveChangesAsync();
            _repository.Remove(foto);
            await _context.SaveChangesAsync();
            var result = await _repository.GetByIdAsync(foto.Id);
            result.Should().BeNull();
        }
    }
}
