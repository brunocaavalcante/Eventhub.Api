using Eventhub.Domain.Entities;
using Eventhub.Domain.Enums;
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
            .Include(n => n.UsuarioDestino)
            .Include(n => n.Evento)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notificacao>> GetNaoLidasByUsuarioAsync(int idUsuario)
    {
        return await _dbSet
            .Where(n => n.IdUsuarioDestino == idUsuario && n.DataLeitura == DateTime.MinValue)
            .Include(n => n.UsuarioOrigem)
            .Include(n => n.UsuarioDestino)
            .Include(n => n.Evento)
            .OrderByDescending(n => n.DataEnvio)
            .ToListAsync();
    }

    public async Task<Notificacao?> GetByIdWithIncludesAsync(int id)
    {
        return await _dbSet
            .Include(n => n.UsuarioOrigem)
            .Include(n => n.UsuarioDestino)
            .Include(n => n.Evento)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task MarcarTodasComoLidasAsync(int idUsuario)
    {
        var notificacoes = await _dbSet
            .Where(n => n.IdUsuarioDestino == idUsuario && n.DataLeitura == DateTime.MinValue)
            .ToListAsync();

        foreach (var notificacao in notificacoes)
        {
            notificacao.Status = EnumNotificacaoStatus.Lida;
            notificacao.DataLeitura = DateTime.UtcNow;
        }

        _context.SaveChanges();
    }
}
