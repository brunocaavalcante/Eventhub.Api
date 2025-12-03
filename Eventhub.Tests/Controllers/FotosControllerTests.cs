using Moq;
using Eventhub.Api.Controllers;
using Eventhub.Application.Interfaces;
using Eventhub.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace Eventhub.Tests.Controllers
{
    public class FotosControllerTests
    {
        private readonly Mock<IFotosService> _serviceMock = new();
        private readonly FotosController _controller;

        public FotosControllerTests()
        {
            _controller = new FotosController(_serviceMock.Object);
        }

        [Fact]
        public async Task Upload_ShouldReturnFotoDto()
        {
            var dto = new UploadFotoDto { NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var fotoDto = new FotoDto { Id = 1 };
            _serviceMock.Setup(s => s.UploadAsync(dto)).ReturnsAsync(fotoDto);

            var result = await _controller.Upload(dto);
            var customResult = result as ObjectResult;
            Assert.NotNull(customResult);
            var response = customResult.Value as Eventhub.Api.Models.CustomResponse<FotoDto>;
            Assert.NotNull(response);
            Assert.NotNull(response.Data);
            response.Data.Id.Should().Be(1);
        }

        [Fact]
        public async Task Update_ShouldReturnFotoDto()
        {
            var dto = new UpdateFotoDto { Id = 1, NomeArquivo = "img.jpg", Base64 = Convert.ToBase64String(new byte[1024]) };
            var fotoDto = new FotoDto { Id = 1 };
            _serviceMock.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(fotoDto);

            var result = await _controller.Update(dto.Id, dto);
            var customResult = result as ObjectResult;
            Assert.NotNull(customResult);
            var response = customResult.Value as Eventhub.Api.Models.CustomResponse<FotoDto>;
            Assert.NotNull(response);
            Assert.NotNull(response.Data);
            response.Data.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_ShouldReturnFotoDto()
        {
            var fotoDto = new FotoDto { Id = 1 };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(fotoDto);

            var result = await _controller.GetById(1);
            var customResult = result as ObjectResult;
            Assert.NotNull(customResult);
            var response = customResult.Value as Eventhub.Api.Models.CustomResponse<FotoDto>;
            Assert.NotNull(response);
            Assert.NotNull(response.Data);
            response.Data.Id.Should().Be(1);
        }

        [Fact]
        public async Task Delete_ShouldReturnOk()
        {
            _serviceMock.Setup(s => s.RemoverAsync(1)).Returns(Task.CompletedTask);
            var result = await _controller.Remover(1);
            var customResult = result as ObjectResult;
            Assert.NotNull(customResult);
            Assert.Equal(204, customResult.StatusCode);
        }
    }
}
