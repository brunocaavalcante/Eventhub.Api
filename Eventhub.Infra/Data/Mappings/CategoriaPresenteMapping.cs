using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class CategoriaPresenteMapping : IEntityTypeConfiguration<CategoriaPresente>
{
    public void Configure(EntityTypeBuilder<CategoriaPresente> builder)
    {
        builder.ToTable("CategoriaPresente");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);
    }
}
