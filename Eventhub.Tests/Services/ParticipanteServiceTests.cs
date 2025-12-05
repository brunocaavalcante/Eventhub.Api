using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Services;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using Moq;

namespace Eventhub.Tests.Services
{
    public class ParticipanteServiceTests
    {
        private readonly Mock<IParticipanteRepository> _participanteRepoMock = new();
        private readonly Mock<IUsuarioRepository> _usuarioRepoMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly ParticipanteService _service;

        public ParticipanteServiceTests()
        {
            _service = new ParticipanteService(
                _participanteRepoMock.Object,
                _usuarioRepoMock.Object,
                _unitOfWorkMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task AdicionarAsync_DeveCriarParticipanteComUsuarioExistente()
        {
            var dto = new CreateParticipanteDto
            {
                IdEvento = 1,
                IdPerfil = 2,
                Nome = "Teste",
                Email = "teste@email.com"
            };
            var usuario = new Usuario { Id = 10, Nome = "Teste", Email = "teste@email.com", Status = "PendenteCadastro" };
            var participante = new Participante { Id = 1, IdEvento = 1, IdPerfil = 2, IdUsuario = 10, Usuario = usuario, CadastroPendente = true, DataCadastro = DateTime.UtcNow };
            var participanteDto = new ParticipanteDto { Id = 1, IdEvento = 1, CadastroPendente = true, Usuario = new UsuarioInfoDto { Id = 10, Nome = "Teste", Email = "teste@email.com" }, Status = "PendenteCadastro" };

            _usuarioRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(usuario);
            _participanteRepoMock.Setup(r => r.ExistsAsync(1, 10, 2, null)).ReturnsAsync(false);
            _participanteRepoMock.Setup(r => r.AddAsync(It.IsAny<Participante>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _participanteRepoMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(participante);
            _mapperMock.Setup(m => m.Map<ParticipanteDto>(It.IsAny<Participante>())).Returns(participanteDto);

            var result = await _service.AdicionarAsync(dto);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.True(result.CadastroPendente);
            Assert.Equal("PendenteCadastro", result.Status);
        }

        [Fact]
        public async Task AdicionarAsync_DeveLancarException_SeParticipanteDuplicado()
        {
            var dto = new CreateParticipanteDto { IdEvento = 1, IdPerfil = 2, Nome = "Teste", Email = "teste@email.com" };
            var usuario = new Usuario { Id = 10, Nome = "Teste", Email = "teste@email.com", Status = "PendenteCadastro" };
            _usuarioRepoMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(usuario);
            _participanteRepoMock.Setup(r => r.ExistsAsync(1, 10, 2, null)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ExceptionValidation>(() => _service.AdicionarAsync(dto));
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarParticipante()
        {
            var dto = new UpdateParticipanteDto { Id = 1, IdPerfil = 2, IdUsuario = 10 };
            var usuario = new Usuario { Id = 10, Nome = "Teste", Email = "teste@email.com", Status = "PendenteCadastro" };
            var participante = new Participante { Id = 1, IdEvento = 1, IdPerfil = 2, IdUsuario = 10, Usuario = usuario, CadastroPendente = true, DataCadastro = DateTime.UtcNow };
            var participanteDto = new ParticipanteDto { Id = 1, IdEvento = 1, CadastroPendente = true, Usuario = new UsuarioInfoDto { Id = 10, Nome = "Teste", Email = "teste@email.com" }, Status = "PendenteCadastro" };

            _participanteRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(participante);
            _usuarioRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(usuario);
            _participanteRepoMock.Setup(r => r.ExistsAsync(1, 10, 2, 1)).ReturnsAsync(false);
            _participanteRepoMock.Setup(r => r.Update(It.IsAny<Participante>()));
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _participanteRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(participante);
            _mapperMock.Setup(m => m.Map<ParticipanteDto>(It.IsAny<Participante>())).Returns(participanteDto);

            var result = await _service.AtualizarAsync(dto);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverParticipante()
        {
            var participante = new Participante { Id = 1, IdEvento = 1, IdPerfil = 2, IdUsuario = 10 };
            _participanteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(participante);
            _participanteRepoMock.Setup(r => r.Remove(participante));
            _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

            await _service.RemoverAsync(1);
            _participanteRepoMock.Verify(r => r.Remove(participante), Times.Once);
        }

        [Fact]
        public async Task ObterPorEventoAsync_DeveRetornarParticipantes()
        {
            var participantes = new List<Participante> { new Participante { Id = 1, IdEvento = 1, IdPerfil = 2, IdUsuario = 10 } };
            var participantesDto = new List<ParticipanteDto> { new ParticipanteDto { Id = 1, IdEvento = 1 } };
            _participanteRepoMock.Setup(r => r.GetByEventoAsync(1)).ReturnsAsync(participantes);
            _mapperMock.Setup(m => m.Map<IEnumerable<ParticipanteDto>>(participantes)).Returns(participantesDto);

            var result = await _service.ObterPorEventoAsync(1);
            Assert.Single(result);
        }

        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarParticipanteDto()
        {
            var participante = new Participante { Id = 1, IdEvento = 1, IdPerfil = 2, IdUsuario = 10 };
            var participanteDto = new ParticipanteDto { Id = 1, IdEvento = 1 };
            _participanteRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(participante);
            _mapperMock.Setup(m => m.Map<ParticipanteDto>(participante)).Returns(participanteDto);

            var result = await _service.ObterPorIdAsync(1);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
    }
}
