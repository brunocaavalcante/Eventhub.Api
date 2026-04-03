using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ParticipantePermissaoMapping : IEntityTypeConfiguration<ParticipantePermissao>
{
    public void Configure(EntityTypeBuilder<ParticipantePermissao> builder)
    {
        builder.ToTable("ParticipantePermissao");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.IdParticipante)
            .IsRequired();

        builder.Property(pp => pp.IdPermissao)
            .IsRequired();

        builder.Property(pp => pp.Concedida)
            .IsRequired();

        // Índice único composto
        builder.HasIndex(pp => new { pp.IdParticipante, pp.IdPermissao })
            .IsUnique();

        // Relacionamentos
        builder.HasOne(pp => pp.Participante)
            .WithMany()
            .HasForeignKey(pp => pp.IdParticipante)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pp => pp.Permissao)
            .WithMany()
            .HasForeignKey(pp => pp.IdPermissao)
            .OnDelete(DeleteBehavior.Restrict);
    }
}