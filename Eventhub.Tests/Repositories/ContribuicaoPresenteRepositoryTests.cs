using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Infra.Data;
using Eventhub.Infra.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Tests.Repositories;

public class ContribuicaoPresenteRepositoryTests
{
    private EventhubDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<EventhubDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new EventhubDbContext(options);
    }

    [Fact]
    public async Task GetTotalContribuidoAsync_DeveSomarContribuicoesDoPresente()
    {
        using var context = CreateInMemoryContext();
        context.ContribuicaoPresentes.AddRange(
            new ContribuicaoPresente { IdPresente = 1, IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado, Valor = 50m },
            new ContribuicaoPresente { IdPresente = 1, IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado, Valor = 25m },
            new ContribuicaoPresente { IdPresente = 2, IdStatusContribuicao = (int)StatusContribuicaoEnum.Confirmado, Valor = 100m }
        );
        await context.SaveChangesAsync();
        var repo = new ContribuicaoPresenteRepository(context);

        var total = await repo.GetTotalContribuidoAsync(1);

        total.Should().Be(75m);
    }

    [Fact]
    public async Task GetTotalContribuidoAsync_DeveRetornarZero_QuandoNaoExistirContribuicao()
    {
        using var context = CreateInMemoryContext();
        var repo = new ContribuicaoPresenteRepository(context);

        var total = await repo.GetTotalContribuidoAsync(1);

        total.Should().Be(0m);
    }
}
