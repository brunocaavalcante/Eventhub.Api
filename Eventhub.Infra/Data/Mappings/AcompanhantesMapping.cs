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

        builder.Property(a => a.IdParticipante)
            .IsRequired();

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(200);

        // Relacionamentos
        builder.HasOne(a => a.Participante)
            .WithMany(p => p.Acompanhantes)
            .HasForeignKey(a => a.IdParticipante)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
