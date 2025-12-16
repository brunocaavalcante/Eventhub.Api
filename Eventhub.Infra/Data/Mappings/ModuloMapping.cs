using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ModuloMapping : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> builder)
    {
        builder.ToTable("Modulo");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Descricao)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(m => m.Icone)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Rota)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Ordem)
            .IsRequired();

        builder.HasMany(m => m.Permissoes)
            .WithOne(p => p.Modulo)
            .HasForeignKey(p => p.IdModulo)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
