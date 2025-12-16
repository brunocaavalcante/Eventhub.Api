using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class EnderecoEventoMapping : IEntityTypeConfiguration<EnderecoEvento>
{
    public void Configure(EntityTypeBuilder<EnderecoEvento> builder)
    {
        builder.ToTable("EnderecoEvento");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.NomeLocal)
            .HasMaxLength(200);

        builder.Property(e => e.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Logradouro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Cep)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PontoReferencia)
            .HasMaxLength(500);

        // Relacionamentos
        builder.HasMany(e => e.Eventos)
            .WithOne(ev => ev.Endereco)
            .HasForeignKey(ev => ev.IdEndereco)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
