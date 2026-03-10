using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PerfilEventoPermissaoMapping : IEntityTypeConfiguration<PerfilEventoPermissao>
{
    public void Configure(EntityTypeBuilder<PerfilEventoPermissao> builder)
    {
        builder.ToTable("PerfilEventoPermissao");

        builder.HasKey(pep => pep.Id);

        builder.Property(pep => pep.IdPerfil)
            .IsRequired();

        builder.Property(pep => pep.IdEvento)
            .IsRequired();

        builder.Property(pep => pep.IdPermissao)
            .IsRequired();

        builder.Property(pep => pep.Concedida)
            .IsRequired();

        // Índice único composto
        builder.HasIndex(pep => new { pep.IdPerfil, pep.IdEvento, pep.IdPermissao })
            .IsUnique();

        // Relacionamentos
        builder.HasOne(pep => pep.Perfil)
            .WithMany()
            .HasForeignKey(pep => pep.IdPerfil)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pep => pep.Evento)
            .WithMany()
            .HasForeignKey(pep => pep.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pep => pep.Permissao)
            .WithMany()
            .HasForeignKey(pep => pep.IdPermissao)
            .OnDelete(DeleteBehavior.Restrict);
    }
}