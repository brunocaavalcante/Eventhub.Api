using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PresenteMapping : IEntityTypeConfiguration<Presente>
{
    public void Configure(EntityTypeBuilder<Presente> builder)
    {
        builder.ToTable("Presente");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdFoto)
            .IsRequired();

        builder.Property(p => p.IdEvento)
            .IsRequired();

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.LinkProduto)
            .HasMaxLength(500);

        builder.Property(p => p.IdStatus)
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(p => p.Foto)
            .WithMany()
            .HasForeignKey(p => p.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Evento)
            .WithMany()
            .HasForeignKey(p => p.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Status)
            .WithMany(s => s.Presentes)
            .HasForeignKey(p => p.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Contribuicoes)
            .WithOne(c => c.Presente)
            .HasForeignKey(c => c.IdPresente)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
