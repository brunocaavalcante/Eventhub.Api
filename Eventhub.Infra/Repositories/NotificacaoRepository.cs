using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Repositories;

public class NotificacaoRepository : Repository<Notificacao>, INotificacaoRepository
{
    public NotificacaoRepository(EventhubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Notificacao>> GetByUsuarioDestinoAsync(int idUsuario)
    {
        return await _dbSet
            .Where(n => n.IdUsuarioDestino == idUsuario)
            .Include(n => n.UsuarioOrigem)
            .Include(n => n.Evento)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacao>> GetNaoLidasByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Where(n => n.IdUsuarioDestino == idUsuario && n.DataLeitura == DateTime.MinValue)
            .Include(n => n.UsuarioOrigem)
            .Include(n => n.Evento)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }
}
