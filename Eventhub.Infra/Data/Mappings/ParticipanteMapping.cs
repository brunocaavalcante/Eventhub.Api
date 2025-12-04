using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ParticipanteMapping : IEntityTypeConfiguration<Participante>
{
    public void Configure(EntityTypeBuilder<Participante> builder)
    {
        builder.ToTable("Participante");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdEvento)
            .IsRequired();

        builder.Property(p => p.IdUsuario)
            .IsRequired();

        builder.Property(p => p.IdPerfil)
            .IsRequired();

        builder.Property(p => p.CadastroPendente)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        builder.HasIndex(p => new { p.IdEvento, p.IdUsuario, p.IdPerfil })
            .IsUnique();

        builder.HasOne(p => p.Evento)
            .WithMany(e => e.Participantes)
            .HasForeignKey(p => p.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Usuario)
            .WithMany(u => u.Participantes)
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Perfil)
            .WithMany(perfil => perfil.Participantes)
            .HasForeignKey(p => p.IdPerfil)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
