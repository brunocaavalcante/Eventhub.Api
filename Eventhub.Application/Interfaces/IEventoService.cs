using Eventhub.Application.DTOs;
using Eventhub.Application.DTOs.Evento;
using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IEventoService
{
    Task<EventoCadastroDto> AdicionarAsync(EventoCadastroDto evento);
    Task<EventoDto> AtualizarAsync(UpdateEventoDto evento);
    Task RemoverAsync(int id);
    Task<EventoDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<EventoAtivoDto>> ObterEventosPorUsuarioAsync(int idUsuario);
    Task<IEnumerable<StatusEventoDto>> ObterStatusEventosAsync();
    Task<EventoAtivoDto?> ObterPorTokenAsync(Guid token);
}
