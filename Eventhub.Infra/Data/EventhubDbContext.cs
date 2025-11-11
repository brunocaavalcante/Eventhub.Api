using Microsoft.EntityFrameworkCore;

namespace Eventhub.Infra.Data;

public class EventhubDbContext : DbContext
{
    public EventhubDbContext(DbContextOptions<EventhubDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventhubDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
