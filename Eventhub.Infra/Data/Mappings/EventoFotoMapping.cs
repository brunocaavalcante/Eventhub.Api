using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class EventoFotoMapping : IEntityTypeConfiguration<EventoFoto>
{
    public void Configure(EntityTypeBuilder<EventoFoto> builder)
    {
        builder.ToTable("EventoFoto");

        builder.HasKey(ef => ef.Id);

        builder.Property(ef => ef.IdFoto)
            .IsRequired();

        builder.Property(ef => ef.Ordem)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(ef => ef.Foto)
            .WithMany(f => f.EventoFotos)
            .HasForeignKey(ef => ef.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ef => ef.Evento)
            .WithMany(e => e.EventoFotos)
            .HasForeignKey(ef => ef.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
