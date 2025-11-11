using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Foto)
            .HasMaxLength(500);

        builder.Property(u => u.Telefone)
            .HasMaxLength(20);

        builder.Property(u => u.DataCadastro)
            .IsRequired();

        builder.Property(u => u.Status)
            .IsRequired()
            .HasMaxLength(50);

        // Relacionamentos
        builder.HasMany(u => u.Eventos)
            .WithOne(e => e.UsuarioCriador)
            .HasForeignKey(e => e.CpfInclusao)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.UsuarioPerfis)
            .WithOne(up => up.Usuario)
            .HasForeignKey(up => up.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.ResponsaveisProgramacao)
            .WithOne(rp => rp.Usuario)
            .HasForeignKey(rp => rp.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.NotificacoesEnviadas)
            .WithOne(n => n.UsuarioOrigem)
            .HasForeignKey(n => n.IdUsuarioOrigem)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.NotificacoesRecebidas)
            .WithOne(n => n.UsuarioDestino)
            .HasForeignKey(n => n.IdUsuarioDestino)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Comentarios)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Curtidas)
            .WithOne(c => c.Usuario)
            .HasForeignKey(c => c.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
