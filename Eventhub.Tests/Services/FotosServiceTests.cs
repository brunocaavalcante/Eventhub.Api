using Moq;
using AutoMapper;
using Eventhub.Application.Services;
using Eventhub.Application.DTOs;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using FluentAssertions;

namespace Eventhub.Tests.Services
{
    public class FotosServiceTests
    {
        private readonly Mock<IFotosRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly FotosService _service;

        public FotosServiceTests()
        {
            _service = new FotosService(_repoMock.Object, _mapperMock.Object, _uowMock.Object);
        }

        [Fact]
        public async Task UploadAsync_ShouldAddAndCommit()
        {
            var dto = new UploadFotoDto { NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var foto = new Fotos { Id = 1, NomeArquivo = dto.NomeArquivo, Base64 = dto.Base64 };
            _mapperMock.Setup(m => m.Map<Fotos>(dto)).Returns(foto);
            _mapperMock.Setup(m => m.Map<FotoDto>(foto)).Returns(new FotoDto { Id = 1 });

            var result = await _service.UploadAsync(dto);

            _repoMock.Verify(r => r.AddAsync(foto), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndCommit()
        {
            var dto = new UpdateFotoDto { Id = 1, NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var foto = new Fotos { Id = 1, NomeArquivo = dto.NomeArquivo, Base64 = dto.Base64 };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(foto);
            _mapperMock.Setup(m => m.Map<FotoDto>(foto)).Returns(new FotoDto { Id = 1 });

            var result = await _service.UpdateAsync(dto);

            _repoMock.Verify(r => r.Update(foto), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task RemoverAsync_ShouldRemoveAndCommit()
        {
            var foto = new Fotos { Id = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(foto);

            await _service.RemoverAsync(1);

            _repoMock.Verify(r => r.Remove(foto), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
