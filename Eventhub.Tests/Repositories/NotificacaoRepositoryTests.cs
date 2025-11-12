using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class NotificacaoRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByUsuarioDestinoAsync_DeveRetornarNotificacoesDoUsuario_OrdenadasPorDataEnvio()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idUsuarioDestino = 1;

        // Adiciona entidades relacionadas
        context.Usuarios.AddRange(
            new Usuario { Id = 1, Nome = "João Silva", Email = "joao@email.com", DataCadastro = DateTime.Now, Status = "Ativo" },
            new Usuario { Id = 2, Nome = "Maria Santos", Email = "maria@email.com", DataCadastro = DateTime.Now, Status = "Ativo" }
        );
        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua A", Numero = "123", Cep = "12345-678", Cidade = "São Paulo", PontoReferencia = "Centro" });
        await context.SaveChangesAsync();

        context.Eventos.Add(new Evento { Id = 1, IdUsuarioCriador = 2, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 });
        await context.SaveChangesAsync();

        var hoje = DateTime.Now;
        context.Notificacoes.AddRange(
            new Notificacao { Id = 1, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = idUsuarioDestino, Titulo = "Notificação 1", Descricao = "Descrição 1", LinkAcao = "", Icone = "icon1", Status = "Enviada", Prioridade = 1, Data = hoje.AddDays(-2), DataCadastro = hoje.AddDays(-2), DataEnvio = hoje.AddDays(-2), DataLeitura = DateTime.MinValue },
            new Notificacao { Id = 2, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = idUsuarioDestino, Titulo = "Notificação 2", Descricao = "Descrição 2", LinkAcao = "", Icone = "icon2", Status = "Enviada", Prioridade = 2, Data = hoje.AddDays(-1), DataCadastro = hoje.AddDays(-1), DataEnvio = hoje.AddDays(-1), DataLeitura = DateTime.MinValue },
            new Notificacao { Id = 3, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = 2, Titulo = "Notificação 3", Descricao = "Descrição 3", LinkAcao = "", Icone = "icon3", Status = "Enviada", Prioridade = 1, Data = hoje, DataCadastro = hoje, DataEnvio = hoje, DataLeitura = DateTime.MinValue }
        );
        await context.SaveChangesAsync();
        var repo = new NotificacaoRepository(context);

        // Act
        var notificacoes = (await repo.GetByUsuarioDestinoAsync(idUsuarioDestino)).ToList();

        // Assert
        notificacoes.Should().HaveCount(2);
        notificacoes.Should().OnlyContain(n => n.IdUsuarioDestino == idUsuarioDestino);
        notificacoes[0].DataEnvio.Should().BeAfter(notificacoes[1].DataEnvio);
        notificacoes.Should().AllSatisfy(n =>
        {
            n.UsuarioOrigem.Should().NotBeNull();
            n.Evento.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task GetNaoLidasByUsuarioAsync_DeveRetornarApenasNotificacoesNaoLidas_OrdenadasPorDataEnvio()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idUsuarioDestino = 1;

        context.Usuarios.AddRange(
            new Usuario { Id = 1, Nome = "João Silva", Email = "joao@email.com", DataCadastro = DateTime.Now, Status = "Ativo" },
            new Usuario { Id = 2, Nome = "Maria Santos", Email = "maria@email.com", DataCadastro = DateTime.Now, Status = "Ativo" }
        );
        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Aniversário", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua B", Numero = "456", Cep = "12345-678", Cidade = "Rio de Janeiro", PontoReferencia = "Centro" });
        await context.SaveChangesAsync();

        context.Eventos.Add(new Evento { Id = 1, IdUsuarioCriador = 2, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 });
        await context.SaveChangesAsync();

        var hoje = DateTime.Now;
        context.Notificacoes.AddRange(
            new Notificacao { Id = 1, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = idUsuarioDestino, Titulo = "Não Lida 1", Descricao = "Descrição 1", LinkAcao = "", Icone = "icon1", Status = "Enviada", Prioridade = 1, Data = hoje.AddDays(-2), DataCadastro = hoje.AddDays(-2), DataEnvio = hoje.AddDays(-2), DataLeitura = DateTime.MinValue },
            new Notificacao { Id = 2, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = idUsuarioDestino, Titulo = "Lida", Descricao = "Descrição 2", LinkAcao = "", Icone = "icon2", Status = "Lida", Prioridade = 2, Data = hoje.AddDays(-1), DataCadastro = hoje.AddDays(-1), DataEnvio = hoje.AddDays(-1), DataLeitura = hoje },
            new Notificacao { Id = 3, IdEvento = 1, IdUsuarioOrigem = 2, IdUsuarioDestino = idUsuarioDestino, Titulo = "Não Lida 2", Descricao = "Descrição 3", LinkAcao = "", Icone = "icon3", Status = "Enviada", Prioridade = 1, Data = hoje, DataCadastro = hoje, DataEnvio = hoje, DataLeitura = DateTime.MinValue }
        );
        await context.SaveChangesAsync();
        var repo = new NotificacaoRepository(context);

        // Act
        var notificacoes = (await repo.GetNaoLidasByUsuarioAsync(idUsuarioDestino)).ToList();

        // Assert
        notificacoes.Should().HaveCount(2);
        notificacoes.Should().OnlyContain(n => n.IdUsuarioDestino == idUsuarioDestino && n.DataLeitura == DateTime.MinValue);
        notificacoes[0].DataEnvio.Should().BeAfter(notificacoes[1].DataEnvio);
        notificacoes.Should().AllSatisfy(n =>
        {
            n.UsuarioOrigem.Should().NotBeNull();
            n.Evento.Should().NotBeNull();
        });
    }
}
