using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class StatusProgramacaoMapping : IEntityTypeConfiguration<StatusProgramacao>
{
    public void Configure(EntityTypeBuilder<StatusProgramacao> builder)
    {
        builder.ToTable("StatusProgramacao");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(100);
    }
}
