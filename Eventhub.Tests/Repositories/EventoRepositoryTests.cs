using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class EventoRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByUsuarioAsync_DeveRetornarEventosDoUsuario_ComRelacionamentos()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idUsuario = 1;

        // Adiciona entidades relacionadas
        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua A", Numero = "123", Cep = "12345-678", Cidade = "São Paulo", PontoReferencia = "Próximo ao metrô" });
        context.Usuarios.Add(new Usuario { Id = 1, Nome = "João Silva", Email = "joao@email.com", DataCadastro = DateTime.Now, Status = "Ativo" });
        await context.SaveChangesAsync();

        context.Eventos.AddRange(
            new Evento { Id = 1, IdUsuarioCriador = idUsuario, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 },
            new Evento { Id = 2, IdUsuarioCriador = idUsuario, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 2", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(2), DataInclusao = DateTime.Now, MaxConvidado = 50 },
            new Evento { Id = 3, IdUsuarioCriador = 2, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 3", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(3), DataInclusao = DateTime.Now, MaxConvidado = 75 }
        );
        await context.SaveChangesAsync();
        var repo = new EventoRepository(context);

        // Act
        var eventos = (await repo.GetEventosByUsuarioAsync(idUsuario)).ToList();

        // Assert
        eventos.Should().HaveCount(2);
        eventos.Should().OnlyContain(e => e.IdUsuarioCriador == idUsuario);
        eventos.Should().AllSatisfy(e =>
        {
            e.TipoEvento.Should().NotBeNull();
            e.Status.Should().NotBeNull();
            e.Endereco.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetByStatusAsync_DeveRetornarEventosDoStatus_ComRelacionamentos()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idStatus = 1;

        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Aniversário", IdFoto = 1 });
        context.StatusEvento.AddRange(
            new StatusEvento { Id = 1, Descricao = "Ativo" },
            new StatusEvento { Id = 2, Descricao = "Cancelado" }
        );
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua B", Numero = "456", Cep = "12345-678", Cidade = "Rio de Janeiro", PontoReferencia = "Centro" });
        context.Usuarios.Add(new Usuario { Id = 1, Nome = "Maria Silva", Email = "maria@email.com", DataCadastro = DateTime.Now, Status = "Ativo" });
        await context.SaveChangesAsync();

        context.Eventos.AddRange(
            new Evento { Id = 1, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = idStatus, IdEndereco = 1, Descricao = "Evento Ativo 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 },
            new Evento { Id = 2, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = idStatus, IdEndereco = 1, Descricao = "Evento Ativo 2", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(2), DataInclusao = DateTime.Now, MaxConvidado = 50 },
            new Evento { Id = 3, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = 2, IdEndereco = 1, Descricao = "Evento Cancelado", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(3), DataInclusao = DateTime.Now, MaxConvidado = 75 }
        );
        await context.SaveChangesAsync();
        var repo = new EventoRepository(context);

        // Act
        var eventos = (await repo.GetByStatusAsync(idStatus)).ToList();

        // Assert
        eventos.Should().HaveCount(2);
        eventos.Should().OnlyContain(e => e.IdStatus == idStatus);
        eventos.Should().AllSatisfy(e =>
        {
            e.TipoEvento.Should().NotBeNull();
            e.Status.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetByTipoAsync_DeveRetornarEventosDoTipo_ComRelacionamentos()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idTipo = 1;

        context.TipoEvento.AddRange(
            new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 },
            new TipoEvento { Id = 2, Descricao = "Formatura", IdFoto = 1 }
        );
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua C", Numero = "789", Cep = "12345-678", Cidade = "Belo Horizonte", PontoReferencia = "Savassi" });
        context.Usuarios.Add(new Usuario { Id = 1, Nome = "Pedro Santos", Email = "pedro@email.com", DataCadastro = DateTime.Now, Status = "Ativo" });
        await context.SaveChangesAsync();

        context.Eventos.AddRange(
            new Evento { Id = 1, IdUsuarioCriador = 1, IdTipoEvento = idTipo, IdStatus = 1, IdEndereco = 1, Descricao = "Casamento 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 },
            new Evento { Id = 2, IdUsuarioCriador = 1, IdTipoEvento = idTipo, IdStatus = 1, IdEndereco = 1, Descricao = "Casamento 2", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(2), DataInclusao = DateTime.Now, MaxConvidado = 50 },
            new Evento { Id = 3, IdUsuarioCriador = 1, IdTipoEvento = 2, IdStatus = 1, IdEndereco = 1, Descricao = "Formatura 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(3), DataInclusao = DateTime.Now, MaxConvidado = 75 }
        );
        await context.SaveChangesAsync();
        var repo = new EventoRepository(context);

        // Act
        var eventos = (await repo.GetByTipoAsync(idTipo)).ToList();

        // Assert
        eventos.Should().HaveCount(2);
        eventos.Should().OnlyContain(e => e.IdTipoEvento == idTipo);
        eventos.Should().AllSatisfy(e =>
        {
            e.TipoEvento.Should().NotBeNull();
            e.Status.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetEventosAtivosByUsuarioAsync_DeveRetornarEventos_OrdenadosPorDataInicio()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idUsuario = 1;

        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Festa", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua D", Numero = "321", Cep = "12345-678", Cidade = "Curitiba", PontoReferencia = "Centro" });
        context.Usuarios.Add(new Usuario { Id = 1, Nome = "Ana Costa", Email = "ana@email.com", DataCadastro = DateTime.Now, Status = "Ativo" });
        await context.SaveChangesAsync();

        var hoje = DateTime.Now;
        context.Eventos.AddRange(
            new Evento { Id = 1, IdUsuarioCriador = idUsuario, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento Futuro 2", DataInicio = hoje.AddDays(10), DataFim = hoje.AddDays(11), DataInclusao = DateTime.Now, MaxConvidado = 100 },
            new Evento { Id = 2, IdUsuarioCriador = idUsuario, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento Futuro 1", DataInicio = hoje.AddDays(5), DataFim = hoje.AddDays(6), DataInclusao = DateTime.Now, MaxConvidado = 50 },
            new Evento { Id = 3, IdUsuarioCriador = idUsuario, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento Passado", DataInicio = hoje.AddDays(-10), DataFim = hoje.AddDays(-9), DataInclusao = DateTime.Now, MaxConvidado = 75 }
        );
        await context.SaveChangesAsync();
        var repo = new EventoRepository(context);

        // Act
        var eventos = (await repo.GetEventosByUsuarioAsync(idUsuario)).ToList();

        // Assert
        eventos.Should().HaveCount(3);
        eventos.Should().OnlyContain(e => e.IdUsuarioCriador == idUsuario);
        eventos[0].DataInicio.Should().BeBefore(eventos[1].DataInicio);
        eventos.Should().AllSatisfy(e =>
        {
            e.TipoEvento.Should().NotBeNull();
            e.Status.Should().NotBeNull();
            e.Endereco.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarEventoComRelacionamentos()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var eventoId = 1;

        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Reunião", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua E", Numero = "654", Cep = "12345-678", Cidade = "Salvador", PontoReferencia = "Centro" });
        context.Usuarios.Add(new Usuario { Id = 1, Nome = "Carlos Lima", Email = "carlos.lima@email.com", DataCadastro = DateTime.Now, Status = "Ativo" });
        await context.SaveChangesAsync();

        context.Eventos.Add(new Evento { Id = eventoId, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Reunião de Trabalho", DataInicio = DateTime.Now.AddDays(1), DataFim = DateTime.Now.AddDays(2), DataInclusao = DateTime.Now, MaxConvidado = 20 });
        await context.SaveChangesAsync();

        var repo = new EventoRepository(context);

        // Act
        var evento = await repo.GetByIdAsync(eventoId);

        // Assert
        evento.Should().NotBeNull();
        evento.TipoEvento.Should().NotBeNull();
        evento.Status.Should().NotBeNull();
        evento.Endereco.Should().NotBeNull();

        evento.Id.Should().Be(eventoId);
        evento.Descricao.Should().Be("Reunião de Trabalho");
    }
}