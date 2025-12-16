using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IProgramacaoEventoService
{
    Task<ProgramacaoEvento> AdicionarAsync(ProgramacaoEvento programacao);
    Task<ProgramacaoEvento> AtualizarAsync(ProgramacaoEvento programacao);
    Task RemoverAsync(int id);
    Task<IEnumerable<ProgramacaoEvento>> ObterPorEventoAsync(int idEvento);
}
