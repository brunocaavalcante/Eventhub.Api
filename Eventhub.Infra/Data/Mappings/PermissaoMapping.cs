using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PermissaoMapping : IEntityTypeConfiguration<Permissao>
{
    public void Configure(EntityTypeBuilder<Permissao> builder)
    {
        builder.ToTable("Permissao");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Modulo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Chave)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Chave)
            .IsUnique();

        // Relacionamentos
        builder.HasMany(p => p.PerfilPermissoes)
            .WithOne(pp => pp.Permissao)
            .HasForeignKey(pp => pp.IdPermissao)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.UsuarioPermissoes)
            .WithOne(up => up.Permissao)
            .HasForeignKey(up => up.IdPermissao)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
