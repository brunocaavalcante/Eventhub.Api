using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Eventhub.Tests.Repositories;

public class ProgramacaoEventoRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByEventoAsync_DeveRetornarProgramacoesDoEvento_OrdenadasPorData()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var idEvento = 1;

        // Adiciona entidades relacionadas
        context.Fotos.AddRange(
            new Fotos { Id = 1, NomeArquivo = "foto1.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" },
            new Fotos { Id = 2, NomeArquivo = "foto2.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" },
            new Fotos { Id = 3, NomeArquivo = "foto3.jpg", DataUpload = DateTime.Now, TamanhoKB = 100, Base64 = "base64" }
        );
        context.Usuarios.AddRange(
            new Usuario { Id = 1, Nome = "João Silva", Email = "joao@email.com", DataCadastro = DateTime.Now, Status = "Ativo" },
            new Usuario { Id = 2, Nome = "Maria Santos", Email = "maria@email.com", DataCadastro = DateTime.Now, Status = "Ativo" }
        );
        context.StatusProgramacao.Add(new StatusProgramacao { Id = 1, Descricao = "Confirmado" });
        context.TipoEvento.Add(new TipoEvento { Id = 1, Descricao = "Casamento", IdFoto = 1 });
        context.StatusEvento.Add(new StatusEvento { Id = 1, Descricao = "Ativo" });
        context.EnderecoEvento.Add(new EnderecoEvento { Id = 1, Logradouro = "Rua A", Numero = "123", Cep = "12345-678", Cidade = "São Paulo", PontoReferencia = "Centro" });
        await context.SaveChangesAsync();

        context.Eventos.AddRange(
            new Evento { Id = 1, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 1", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(1), DataInclusao = DateTime.Now, MaxConvidado = 100 },
            new Evento { Id = 2, IdUsuarioCriador = 1, IdTipoEvento = 1, IdStatus = 1, IdEndereco = 1, Descricao = "Evento 2", DataInicio = DateTime.Now, DataFim = DateTime.Now.AddDays(2), DataInclusao = DateTime.Now, MaxConvidado = 50 }
        );
        await context.SaveChangesAsync();

        var hoje = DateTime.Now;
        context.ProgramacaoEvento.AddRange(
            new ProgramacaoEvento { Id = 1, IdEvento = idEvento, IdFoto = 1, IdStatus = 1, Titulo = "Programação 2", Descricao = "Segunda programação", Data = hoje.AddDays(2), Duracao = TimeSpan.FromHours(2), Local = "Salão B", Responsavel = "João", DataCadastro = DateTime.Now },
            new ProgramacaoEvento { Id = 2, IdEvento = idEvento, IdFoto = 2, IdStatus = 1, Titulo = "Programação 1", Descricao = "Primeira programação", Data = hoje.AddDays(1), Duracao = TimeSpan.FromHours(1), Local = "Salão A", Responsavel = "Maria", DataCadastro = DateTime.Now },
            new ProgramacaoEvento { Id = 3, IdEvento = 2, IdFoto = 3, IdStatus = 1, Titulo = "Programação Outro Evento", Descricao = "Programação de outro evento", Data = hoje.AddDays(1), Duracao = TimeSpan.FromHours(3), Local = "Salão C", Responsavel = "Pedro", DataCadastro = DateTime.Now }
        );
        await context.SaveChangesAsync();

        // Adiciona responsáveis
        context.ResponsavelProgramacao.AddRange(
            new ResponsavelProgramacao { Id = 1, IdProgramacao = 1, IdUsuario = 1, Funcao = "Coordenador" },
            new ResponsavelProgramacao { Id = 2, IdProgramacao = 2, IdUsuario = 2, Funcao = "Assistente" }
        );
        await context.SaveChangesAsync();

        var repo = new ProgramacaoEventoRepository(context);

        // Act
        var programacoes = (await repo.GetByEventoAsync(idEvento)).ToList();

        // Assert
        programacoes.Should().HaveCount(2);
        programacoes.Should().OnlyContain(p => p.IdEvento == idEvento);
        programacoes[0].Data.Should().BeBefore(programacoes[1].Data);
        programacoes.Should().AllSatisfy(p =>
        {
            p.Foto.Should().NotBeNull();
            p.Responsaveis.Should().NotBeNull();
        });
    }
}
