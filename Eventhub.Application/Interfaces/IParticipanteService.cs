using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IParticipanteService
{
    Task<ParticipanteDto> AdicionarAsync(CreateParticipanteDto participanteDto);
    Task<ParticipanteDto> AtualizarAsync(UpdateParticipanteDto participanteDto);
    Task RemoverAsync(int id);
    Task<IEnumerable<ParticipanteDto>> ObterPorEventoAsync(int idEvento);
    Task<ParticipanteDto?> ObterPorIdAsync(int id);
}
