using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class TipoEventoRepository : Repository<TipoEvento>, ITipoEventoRepository
{
    public TipoEventoRepository(EventhubDbContext context) : base(context)
    {
    }
}
