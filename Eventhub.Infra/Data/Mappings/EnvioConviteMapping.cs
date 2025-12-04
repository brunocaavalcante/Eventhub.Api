using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class EnvioConviteMapping : IEntityTypeConfiguration<EnvioConvite>
{
    public void Configure(EntityTypeBuilder<EnvioConvite> builder)
    {
        builder.ToTable("EnvioConvite");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.IdConvite)
            .IsRequired();

        builder.Property(e => e.IdParticipante)
            .IsRequired();

        builder.Property(e => e.IdEvento)
            .IsRequired();

        builder.Property(e => e.DataEnvio)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.MensagemResposta)
            .HasMaxLength(1000);

        builder.Property(e => e.QtdAcompanhantes)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(e => e.Convite)
            .WithMany(c => c.EnviosConvite)
            .HasForeignKey(e => e.IdConvite)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Participante)
            .WithMany(p => p.EnviosConvite)
            .HasForeignKey(e => e.IdParticipante)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Evento)
            .WithMany(ev => ev.EnviosConvite)
            .HasForeignKey(e => e.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
