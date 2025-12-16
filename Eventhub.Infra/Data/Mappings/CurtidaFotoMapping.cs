using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class CurtidaFotoMapping : IEntityTypeConfiguration<CurtidaFoto>
{
    public void Configure(EntityTypeBuilder<CurtidaFoto> builder)
    {
        builder.ToTable("CurtidaFoto");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IdFoto)
            .IsRequired();

        builder.Property(c => c.IdUsuario)
            .IsRequired();

        builder.Property(c => c.Data)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(c => c.Foto)
            .WithMany(f => f.Curtidas)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Curtidas)
            .HasForeignKey(c => c.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único para evitar curtida duplicada
        builder.HasIndex(c => new { c.IdFoto, c.IdUsuario })
            .IsUnique();
    }
}
