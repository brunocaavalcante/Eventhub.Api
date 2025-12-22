using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class PresenteRepository : Repository<Presente>, IPresenteRepository
{
    public PresenteRepository(EventhubDbContext context) : base(context) { }
}