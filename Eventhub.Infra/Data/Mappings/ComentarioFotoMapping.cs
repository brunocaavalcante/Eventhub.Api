using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ComentarioFotoMapping : IEntityTypeConfiguration<ComentarioFoto>
{
    public void Configure(EntityTypeBuilder<ComentarioFoto> builder)
    {
        builder.ToTable("ComentarioFoto");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IdFoto)
            .IsRequired();

        builder.Property(c => c.IdUsuario)
            .IsRequired();

        builder.Property(c => c.Data)
            .IsRequired();

        builder.Property(c => c.Comentario)
            .IsRequired()
            .HasMaxLength(1000);

        // Relacionamentos
        builder.HasOne(c => c.Foto)
            .WithMany(f => f.Comentarios)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
