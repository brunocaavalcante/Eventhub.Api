using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class StatusEventoRepository : Repository<StatusEvento>, IStatusEventoRepository
{
    public StatusEventoRepository(EventhubDbContext context) : base(context)
    {
    }
}
