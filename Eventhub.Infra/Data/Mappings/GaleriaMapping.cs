using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class GaleriaMapping : IEntityTypeConfiguration<Galeria>
{
    public void Configure(EntityTypeBuilder<Galeria> builder)
    {
        builder.ToTable("Galeria");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.IdEvento)
            .IsRequired();

        builder.Property(g => g.IdFoto)
            .IsRequired();

        builder.Property(g => g.Ordem)
            .IsRequired();

        builder.Property(g => g.Visibilidade)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(g => g.Legenda)
            .HasMaxLength(500);

        builder.Property(g => g.Data)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(g => g.Evento)
            .WithMany(e => e.Galerias)
            .HasForeignKey(g => g.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Foto)
            .WithMany(f => f.Galerias)
            .HasForeignKey(g => g.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
