using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class TipoEventoMapping : IEntityTypeConfiguration<TipoEvento>
{
    public void Configure(EntityTypeBuilder<TipoEvento> builder)
    {
        builder.ToTable("TipoEvento");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.IdFoto)
            .IsRequired();

        builder.Property(t => t.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Icon)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasOne(t => t.Foto)
            .WithMany(f => f.TipoEventos)
            .HasForeignKey(t => t.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Eventos)
            .WithOne(e => e.TipoEvento)
            .HasForeignKey(e => e.IdTipoEvento)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
