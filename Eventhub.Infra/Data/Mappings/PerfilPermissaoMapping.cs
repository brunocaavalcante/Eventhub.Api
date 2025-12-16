using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PerfilPermissaoMapping : IEntityTypeConfiguration<PerfilPermissao>
{
    public void Configure(EntityTypeBuilder<PerfilPermissao> builder)
    {
        builder.ToTable("PerfilPermissao");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.IdPerfil)
            .IsRequired();

        builder.Property(pp => pp.IdPermissao)
            .IsRequired();

        builder.HasIndex(pp => new { pp.IdPerfil, pp.IdPermissao })
            .IsUnique();

        // Relacionamentos
        builder.HasOne(pp => pp.Perfil)
            .WithMany(p => p.PerfilPermissoes)
            .HasForeignKey(pp => pp.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Permissao)
            .WithMany(p => p.PerfilPermissoes)
            .HasForeignKey(pp => pp.IdPermissao)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
