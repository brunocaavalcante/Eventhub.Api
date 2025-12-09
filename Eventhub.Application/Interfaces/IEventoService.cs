using Eventhub.Application.DTOs;
using Eventhub.Domain.Entities;

namespace Eventhub.Application.Interfaces;

public interface IEventoService
{
    Task<EventoCadastroDto> AdicionarAsync(EventoCadastroDto evento);
    Task<Evento> AtualizarAsync(Evento evento);
    Task RemoverAsync(int id);
    Task<EventoDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<EventoAtivoDto>> ObterEventosPorUsuarioAsync(int idUsuario);
    Task<IEnumerable<StatusEventoDto>> ObterStatusEventosAsync();
}
