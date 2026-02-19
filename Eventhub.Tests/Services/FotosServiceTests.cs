using Moq;
using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Services;
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
        private readonly Mock<IImageStorageService> _storageMock = new();
        private readonly FotosService _service;

        public FotosServiceTests()
        {
            _service = new FotosService(_repoMock.Object, _mapperMock.Object, _uowMock.Object, _storageMock.Object);
        }

        [Fact]
        public async Task UploadAsync_ShouldAddAndCommit()
        {
            var dto = new UploadFotoDto { NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var foto = new Fotos { Id = 1 };
            _storageMock.Setup(s => s.UploadAsync(It.IsAny<ImageStorageUploadRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageStorageUploadResult
                {
                    PublicId = "pid",
                    Url = "https://cdn/img.jpg",
                    Bytes = 1024,
                    ContentType = "image/jpeg"
                });
            _mapperMock.Setup(m => m.Map<FotoDto>(It.IsAny<Fotos>())).Returns(new FotoDto { Id = 1 });

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Fotos>()))
                .Callback<Fotos>(f => foto = f)
                .Returns(Task.CompletedTask);

            var result = await _service.UploadAsync(dto);

            _repoMock.Verify(r => r.AddAsync(It.IsAny<Fotos>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndCommit()
        {
            var dto = new UpdateFotoDto { Id = 1, NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var foto = new Fotos { Id = 1, NomeArquivo = dto.NomeArquivo, Url = "old", PublicId = "old" };
            _repoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(foto);
            _storageMock.Setup(s => s.UploadAsync(It.IsAny<ImageStorageUploadRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ImageStorageUploadResult
                {
                    PublicId = "pid",
                    Url = "https://cdn/new.jpg",
                    Bytes = 2048,
                    ContentType = "image/jpeg"
                });
            _mapperMock.Setup(m => m.Map<FotoDto>(foto)).Returns(new FotoDto { Id = 1 });

            var result = await _service.UpdateAsync(dto);

            _repoMock.Verify(r => r.Update(foto), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _storageMock.Verify(s => s.DeleteAsync("old"), Times.Once);
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task RemoverAsync_ShouldRemoveAndCommit()
        {
            var foto = new Fotos { Id = 1, PublicId = "pid" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(foto);

            await _service.RemoverAsync(1);

            _repoMock.Verify(r => r.Remove(foto), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _storageMock.Verify(s => s.DeleteAsync("pid"), Times.Once);
        }
    }
}
