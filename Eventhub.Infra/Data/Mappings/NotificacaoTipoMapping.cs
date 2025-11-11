using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class NotificacaoTipoMapping : IEntityTypeConfiguration<NotificacaoTipo>
{
    public void Configure(EntityTypeBuilder<NotificacaoTipo> builder)
    {
        builder.ToTable("NotificacaoTipo");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Descricao)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(n => n.TextoPadrao)
            .HasMaxLength(500);

        builder.Property(n => n.IconePadrao)
            .HasMaxLength(100);

        // Relacionamentos
        builder.HasMany(n => n.Notificacoes)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
