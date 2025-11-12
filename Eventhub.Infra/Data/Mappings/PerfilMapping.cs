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

        // Relacionamentos
        builder.HasMany(p => p.UsuarioPerfis)
            .WithOne(up => up.Perfil)
            .HasForeignKey(up => up.IdPerfil)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
