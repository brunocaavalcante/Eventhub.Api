using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class StatusContribuicaoMapping : IEntityTypeConfiguration<StatusContribuicao>
{
    public void Configure(EntityTypeBuilder<StatusContribuicao> builder)
    {
        builder.ToTable("StatusContribuicao");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasMany(s => s.Contribuicoes)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
