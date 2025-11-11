using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class StatusEventoMapping : IEntityTypeConfiguration<StatusEvento>
{
    public void Configure(EntityTypeBuilder<StatusEvento> builder)
    {
        builder.ToTable("StatusEvento");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasMany(s => s.Eventos)
            .WithOne(e => e.Status)
            .HasForeignKey(e => e.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
