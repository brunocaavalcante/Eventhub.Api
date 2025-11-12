using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class UsuarioPerfilEventoMapping : IEntityTypeConfiguration<UsuarioPerfilEvento>
{
    public void Configure(EntityTypeBuilder<UsuarioPerfilEvento> builder)
    {
        builder.ToTable("UsuarioPerfilEvento");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.IdEvento)
            .IsRequired();

        builder.Property(up => up.IdUsuario)
            .IsRequired();

        builder.Property(up => up.IdPerfil)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(up => up.Evento)
            .WithMany(e => e.UsuarioPerfis)
            .HasForeignKey(up => up.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Usuario)
            .WithMany(u => u.UsuarioPerfis)
            .HasForeignKey(up => up.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Perfil)
            .WithMany(p => p.UsuarioPerfis)
            .HasForeignKey(up => up.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único para evitar duplicação
        builder.HasIndex(up => new { up.IdEvento, up.IdUsuario, up.IdPerfil })
            .IsUnique();
    }
}
