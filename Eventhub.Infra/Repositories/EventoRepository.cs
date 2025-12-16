using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class EventoRepository : Repository<Evento>, IEventoRepository
{
    public EventoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<Evento?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(e => e.TipoEvento)
            .Include(e => e.Status)
            .Include(e => e.Endereco)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Evento>> GetByStatusAsync(int idStatus)
    {
        return await _dbSet
            .Where(e => e.IdStatus == idStatus)
            .Include(e => e.TipoEvento)
            .Include(e => e.Status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetByTipoAsync(int idTipo)
    {
        return await _dbSet
            .Where(e => e.IdTipoEvento == idTipo)
            .Include(e => e.TipoEvento)
            .Include(e => e.Status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Evento>> GetEventosByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Where(e => e.IdUsuarioCriador == idUsuario)
            .Include(e => e.TipoEvento)
            .Include(e => e.Status)
            .Include(e => e.Galerias).ThenInclude(g => g.Foto)
            .OrderBy(e => e.DataInicio)
            .ToListAsync();
    }
}
