using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ResponsavelProgramacaoMapping : IEntityTypeConfiguration<ResponsavelProgramacao>
{
    public void Configure(EntityTypeBuilder<ResponsavelProgramacao> builder)
    {
        builder.ToTable("ResponsavelProgramacao");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.IdProgramacao)
            .IsRequired();

        builder.Property(r => r.IdUsuario)
            .IsRequired();

        builder.Property(r => r.Funcao)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasOne(r => r.Programacao)
            .WithMany(p => p.Responsaveis)
            .HasForeignKey(r => r.IdProgramacao)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Usuario)
            .WithMany(u => u.ResponsaveisProgramacao)
            .HasForeignKey(r => r.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
