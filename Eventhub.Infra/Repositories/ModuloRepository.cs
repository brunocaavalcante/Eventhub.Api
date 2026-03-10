using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class ModuloRepository : Repository<Modulo>, IModuloRepository
{
    public ModuloRepository(EventhubDbContext context) : base(context)
    {
    }
}
