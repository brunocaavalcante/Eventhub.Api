using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class StatusPresenteMapping : IEntityTypeConfiguration<StatusPresente>
{
    public void Configure(EntityTypeBuilder<StatusPresente> builder)
    {
        builder.ToTable("StatusPresente");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasMany(s => s.Presentes)
            .WithOne(p => p.Status)
            .HasForeignKey(p => p.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
