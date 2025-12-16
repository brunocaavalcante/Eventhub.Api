using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories
{
    public class ParticipanteRepositoryTests
    {
        private EventhubDbContext GetDbContext()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<EventhubDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new EventhubDbContext(options);
        }

        [Fact]
        public async Task GetByEventoAsync_DeveRetornarParticipantesPorEvento()
        {
            using var context = GetDbContext();
            context.Perfis.Add(new Perfil { Id = 2, Descricao = "Perfil" });
            context.Usuarios.Add(new Usuario { Id = 10, Nome = "Alice", Email = "alice@email.com" });
            context.Usuarios.Add(new Usuario { Id = 11, Nome = "Bob", Email = "bob@email.com" });
            context.Participantes.Add(new Participante { Id = 1, IdEvento = 1, IdUsuario = 10, IdPerfil = 2 });
            context.Participantes.Add(new Participante { Id = 2, IdEvento = 1, IdUsuario = 11, IdPerfil = 2 });
            context.SaveChanges();
            var repo = new ParticipanteRepository(context);

            var result = await repo.GetByEventoAsync(1);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_DeveRetornarParticipanteComUsuarioPerfil()
        {
            using var context = GetDbContext();
            var usuario = new Usuario { Id = 10, Nome = "Teste", Email = "teste@email.com" };
            var perfil = new Perfil { Id = 2, Descricao = "Perfil" };
            var participante = new Participante { Id = 1, IdEvento = 1, IdUsuario = 10, IdPerfil = 2, Usuario = usuario, Perfil = perfil };
            context.Usuarios.Add(usuario);
            context.Perfis.Add(perfil);
            context.Participantes.Add(participante);
            context.SaveChanges();
            var repo = new ParticipanteRepository(context);

            var result = await repo.GetByIdWithDetailsAsync(1);
            Assert.NotNull(result);
            Assert.Equal(10, result.Usuario.Id);
            Assert.Equal(2, result.Perfil.Id);
        }

        [Fact]
        public async Task ExistsAsync_DeveRetornarTrue_SeParticipanteExiste()
        {
            using var context = GetDbContext();
            context.Participantes.Add(new Participante { Id = 1, IdEvento = 1, IdUsuario = 10, IdPerfil = 2 });
            context.SaveChanges();
            var repo = new ParticipanteRepository(context);

            var exists = await repo.ExistsAsync(1, 10, 2);
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsAsync_DeveRetornarFalse_SeParticipanteNaoExiste()
        {
            using var context = GetDbContext();
            var repo = new ParticipanteRepository(context);
            var exists = await repo.ExistsAsync(1, 10, 2);
            Assert.False(exists);
        }
    }
}
