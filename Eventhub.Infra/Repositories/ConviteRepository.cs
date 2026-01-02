using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class ConviteRepository : Repository<Convite>, IConviteRepository
{
    public ConviteRepository(EventhubDbContext context) : base(context) {}
}
