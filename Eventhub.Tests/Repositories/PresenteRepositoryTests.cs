using Eventhub.Domain.Entities;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class PresenteRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPresente_QuandoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Presentes.Add(new Presente 
        { 
            Id = 1, 
            Nome = "Notebook", 
            Descricao = "Notebook Dell",
            Valor = 3000
        });
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presente = await repo.GetByIdAsync(1);

        // Assert
        presente.Should().NotBeNull();
        presente!.Nome.Should().Be("Notebook");
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosOsPresentes()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        context.Presentes.AddRange(
            new Presente { Id = 1, Nome = "Presente 1", Valor = 100, Descricao = "Desc 1" },
            new Presente { Id = 2, Nome = "Presente 2", Valor = 200, Descricao = "Desc 2" }
        );
        await context.SaveChangesAsync();
        var repo = new PresenteRepository(context);

        // Act
        var presentes = (await repo.GetAllAsync()).ToList();

        // Assert
        presentes.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdDetalhesAsync_DeveRetornarNull_QuandoNaoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repo = new PresenteRepository(context);

        // Act
        var presente = await repo.GetByIdDetalhesAsync(999);

        // Assert
        presente.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdDetalhesAsync_DeveRetornarPresenteComContribuicoes_QuandoExistir()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var categoria = new CategoriaPresente { Id = 1, Nome = "Eletrônicos" };
        var status = new StatusPresente { Id = 1, Descricao = "Disponível" };
        var statusContribuicao = new StatusContribuicao { Id = 1, Descricao = "Confirmado" };
        var foto = new Fotos
        {
            Id = 1,
            NomeArquivo = "foto.jpg",
            Url = "https://cdn/foto.jpg",
            ContentType = "image/jpeg"
        };

        var usuario = new Usuario
        {
            Id = 1,
            Nome = "João Silva",
            Email = "joao@email.com",
            Telefone = "11999999999",
            IdFoto = foto.Id,
            Foto = foto
        };

        var evento = new Evento
        {
            Id = 1,
            Nome = "Casamento",
            DataInicio = DateTime.UtcNow
        };

        var participante = new Participante
        {
            Id = 1,
            IdEvento = evento.Id,
            IdUsuario = usuario.Id,
            Usuario = usuario,
            Evento = evento
        };

        var presente = new Presente
        {
            Id = 1,
            Nome = "Notebook",
            Descricao = "Dell Inspiron",
            Valor = 3000,
            IdEvento = evento.Id,
            IdCategoria = categoria.Id,
            IdStatus = status.Id,
            Categoria = categoria,
            Status = status
        };

        var contribuicao = new ContribuicaoPresente
        {
            Id = 1,
            IdPresente = presente.Id,
            IdParticipante = participante.Id,
            IdStatusContribuicao = statusContribuicao.Id,
            IdFoto = foto.Id,
            Valor = 500,
            FormaPagamento = "Pix",
            DataCadastro = DateTime.UtcNow,
            Presente = presente,
            Participante = participante,
            StatusContribuicao = statusContribuicao,
            Foto = foto
        };

        context.Fotos.Add(foto);
        context.Usuarios.Add(usuario);
        context.Eventos.Add(evento);
        context.Participantes.Add(participante);
        context.CategoriaPresentes.Add(categoria);
        context.StatusContribuicoes.Add(statusContribuicao);
        context.Presentes.Add(presente);
        context.ContribuicaoPresentes.Add(contribuicao);
        await context.SaveChangesAsync();

        var repo = new PresenteRepository(context);

        // Act
        var result = await repo.GetByIdDetalhesAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Nome.Should().Be("Notebook");
        result.Categoria.Should().NotBeNull();
        result.Categoria.Nome.Should().Be("Eletrônicos");
        result.Contribuicoes.Should().HaveCount(1);
        result.Contribuicoes.First().Participante.Should().NotBeNull();
        result.Contribuicoes.First().Participante.Usuario.Should().NotBeNull();
        result.Contribuicoes.First().Participante.Usuario.Nome.Should().Be("João Silva");
    }
}