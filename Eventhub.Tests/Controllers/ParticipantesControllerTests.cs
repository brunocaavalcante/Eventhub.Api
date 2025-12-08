using Eventhub.Api.Controllers;
using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Eventhub.Tests.Controllers
{
    public class ParticipantesControllerTests
    {
        private readonly Mock<IParticipanteService> _serviceMock = new();
        private readonly Mock<IEnvioConviteService> _envioConviteServiceMock = new();
        private readonly ParticipantesController _controller;

        public ParticipantesControllerTests()
        {
            _controller = new ParticipantesController(_serviceMock.Object, _envioConviteServiceMock.Object);
        }

        [Fact]
        public async Task ObterPorEvento_DeveRetornarParticipantes()
        {
            var participantes = new List<ParticipanteDto> { new ParticipanteDto { Id = 1, IdEvento = 1 } };
            _serviceMock.Setup(s => s.ObterPorEventoAsync(1)).ReturnsAsync(participantes);

            var result = await _controller.ObterPorEvento(1);
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<CustomResponse<IEnumerable<ParticipanteDto>>>(okResult.Value);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarParticipante_SeEncontrado()
        {
            var participante = new ParticipanteDto { Id = 1, IdEvento = 1 };
            _serviceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync(participante);

            var result = await _controller.ObterPorId(1);
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<CustomResponse<ParticipanteDto>>(okResult.Value);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task ObterPorId_DeveRetornarNotFound_SeNaoEncontrado()
        {
            _serviceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync((ParticipanteDto?)null);
            var result = await _controller.ObterPorId(1);
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Criar_DeveRetornarParticipanteCriado()
        {
            var dto = new CreateParticipanteDto { IdEvento = 1, IdPerfil = 2, Nome = "Teste", Email = "teste@email.com" };
            var participante = new ParticipanteDto { Id = 1, IdEvento = 1 };
            _serviceMock.Setup(s => s.AdicionarAsync(dto)).ReturnsAsync(participante);

            var result = await _controller.Criar(dto);
            var createdResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<CustomResponse<ParticipanteDto>>(createdResult.Value);
            Assert.Equal(1, response.Data.Id);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarParticipanteAtualizado()
        {
            var dto = new UpdateParticipanteDto { Id = 1, IdPerfil = 2, IdUsuario = 10 };
            var participante = new ParticipanteDto { Id = 1, IdEvento = 1 };
            _serviceMock.Setup(s => s.AtualizarAsync(dto)).ReturnsAsync(participante);

            var result = await _controller.Atualizar(1, dto);
            var okResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<CustomResponse<ParticipanteDto>>(okResult.Value);
            Assert.Equal(1, response.Data.Id);
        }
    }
}
