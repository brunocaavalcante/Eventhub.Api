using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class PixEventoMapping : IEntityTypeConfiguration<PixEvento>
{
    public void Configure(EntityTypeBuilder<PixEvento> builder)
    {
        builder.ToTable("PixEvento");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdEvento)
            .IsRequired();

        builder.Property(p => p.Finalidade)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.NomeBeneficiario)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.QRCodePix)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        // Relacionamento com Evento
        builder.HasOne(p => p.Evento)
            .WithMany()
            .HasForeignKey(p => p.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(p => new { p.IdEvento, p.Finalidade })
            .IsUnique()
            .HasDatabaseName("IX_PixEvento_IdEvento_Finalidade");

        // Índice para busca por evento
        builder.HasIndex(p => p.IdEvento)
            .HasDatabaseName("IX_PixEvento_IdEvento");
    }
}
