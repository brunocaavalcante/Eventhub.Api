using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class AcompanhantesMapping : IEntityTypeConfiguration<Acompanhantes>
{
    public void Configure(EntityTypeBuilder<Acompanhantes> builder)
    {
        builder.ToTable("Acompanhantes");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.IdConvite)
            .IsRequired();

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(200);

        // Relacionamentos
        builder.HasOne(a => a.Convidado)
            .WithMany(c => c.Acompanhantes)
            .HasForeignKey(a => a.IdConvite)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
