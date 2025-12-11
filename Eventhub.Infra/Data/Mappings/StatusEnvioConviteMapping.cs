using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class StatusEnvioConviteMapping : IEntityTypeConfiguration<StatusEnvioConvite>
{
    public void Configure(EntityTypeBuilder<StatusEnvioConvite> builder)
    {
        builder.ToTable("StatusEnvioConvite");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Descricao)
            .IsRequired()
            .HasMaxLength(100);
    }
}
