using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class ParticipanteRepository : Repository<Participante>, IParticipanteRepository
{
    public ParticipanteRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Participante>> GetByEventoAsync(int idEvento)
    {
        return await _dbSet
            .Where(p => p.IdEvento == idEvento)
            .Include(p => p.Usuario)
            .Include(p => p.Perfil)
            .OrderBy(p => p.Usuario.Nome)
            .ToListAsync();
    }

    public async Task<Participante?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Usuario)
            .Include(p => p.Perfil)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Participante?> GetByUsuarioEventoWithDetailsAsync(int idParticipante, int idEvento)
    {
        return await _dbSet
            .Include(p => p.Usuario)
            .Include(p => p.Perfil)
            .FirstOrDefaultAsync(p => p.IdUsuario == idParticipante && p.IdEvento == idEvento);
    }

    public async Task<IEnumerable<Participante>?> ObterConvidadoAcompanhantesPorEvento(int idEvento)
    {
        return await _dbSet
            .Include(p => p.Usuario).ThenInclude(f => f.Foto)
            .Include(p => p.EnviosConvite)
            .Where(p => p.IdEvento == idEvento && p.IdPerfil == (int)EnumPerfil.Convidado)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int idEvento, int idUsuario, int idPerfil, int? ignoreId = null)
    {
        var query = _dbSet.Where(p => p.IdEvento == idEvento && p.IdUsuario == idUsuario && p.IdPerfil == idPerfil);
        if (ignoreId.HasValue)
        {
            query = query.Where(p => p.Id != ignoreId.Value);
        }

        return await query.AnyAsync();
    }
}
