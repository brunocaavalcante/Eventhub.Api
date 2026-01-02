using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class UsuarioPermissaoMapping : IEntityTypeConfiguration<UsuarioPermissao>
{
    public void Configure(EntityTypeBuilder<UsuarioPermissao> builder)
    {
        builder.ToTable("UsuarioPermissao");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.IdUsuario)
            .IsRequired();

        builder.Property(up => up.IdPermissao)
            .IsRequired();

        builder.Property(up => up.Concedida)
            .IsRequired();

        builder.HasIndex(up => new { up.IdUsuario, up.IdPermissao })
            .IsUnique();

        // Relacionamentos
        builder.HasOne(up => up.Usuario)
            .WithMany(u => u.UsuarioPermissoes)
            .HasForeignKey(up => up.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permissao)
            .WithMany(p => p.UsuarioPermissoes)
            .HasForeignKey(up => up.IdPermissao)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
