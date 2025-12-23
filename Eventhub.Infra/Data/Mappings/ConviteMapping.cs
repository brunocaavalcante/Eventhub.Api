using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ConviteMapping : IEntityTypeConfiguration<Convite>
{
    public void Configure(EntityTypeBuilder<Convite> builder)
    {
        builder.ToTable("Convite");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Nome2)
            .HasMaxLength(200);

        builder.Property(c => c.Mensagem)
            .HasMaxLength(2000);

        builder.Property(c => c.TemaConvite)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.IdFoto)
            .IsRequired();
            
        builder.Property(c => c.Opacity)
            .IsRequired();
        
        builder.Property(c => c.DataInicio)
            .IsRequired(false);
        
        builder.Property(c => c.DataFim)
            .IsRequired(false);
        
        builder.Property(c => c.DataCriacao)
            .IsRequired();
        
        builder.Property(c => c.IdEvento)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(c => c.Foto)
            .WithMany(f => f.Convites)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.EnviosConvite)
            .WithOne(ec => ec.Convite)
            .HasForeignKey(ec => ec.IdConvite)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.Evento)
            .WithOne(e => e.Convite)
            .HasForeignKey<Convite>(c => c.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
