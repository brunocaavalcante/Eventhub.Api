using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class FotosMapping : IEntityTypeConfiguration<Fotos>
{
    public void Configure(EntityTypeBuilder<Fotos> builder)
    {
        builder.ToTable("Fotos");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.NomeArquivo)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.DataUpload)
            .IsRequired();

        builder.Property(f => f.TamanhoKB)
            .IsRequired();

        builder.Property(f => f.Base64)
            .IsRequired()
            .HasColumnType("LONGTEXT");

        // Relacionamentos
        builder.HasMany(f => f.TipoEventos)
            .WithOne(t => t.Foto)
            .HasForeignKey(t => t.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.EventoFotos)
            .WithOne(ef => ef.Foto)
            .HasForeignKey(ef => ef.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Galerias)
            .WithOne(g => g.Foto)
            .HasForeignKey(g => g.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Programacoes)
            .WithOne(p => p.Foto)
            .HasForeignKey(p => p.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Convidados)
            .WithOne(c => c.Foto)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Convites)
            .WithOne(c => c.Foto)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Comentarios)
            .WithOne(c => c.Foto)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Curtidas)
            .WithOne(c => c.Foto)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
