using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PerfilMapping : IEntityTypeConfiguration<Perfil>
{
    public void Configure(EntityTypeBuilder<Perfil> builder)
    {
        builder.ToTable("Perfil");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Icon)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(1)
            .HasDefaultValue('A');

        // Relacionamentos
        builder.HasMany(p => p.UsuarioPerfis)
            .WithOne(up => up.Perfil)
            .HasForeignKey(up => up.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.PerfilPermissoes)
            .WithOne(pp => pp.Perfil)
            .HasForeignKey(pp => pp.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
