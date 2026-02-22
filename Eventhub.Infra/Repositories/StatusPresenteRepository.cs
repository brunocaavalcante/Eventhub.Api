using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class StatusPresenteRepository : Repository<StatusPresente>, IStatusPresenteRepository
{
    public StatusPresenteRepository(EventhubDbContext context) : base(context)
    {
    }
}
