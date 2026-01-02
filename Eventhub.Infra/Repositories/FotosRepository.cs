using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Eventhub.Infra.Data;

namespace Eventhub.Infra.Repositories;

public class FotosRepository : Repository<Fotos>, IFotosRepository
{
    public FotosRepository(EventhubDbContext context) : base(context) { }
}
