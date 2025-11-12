using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ConvidadosMapping : IEntityTypeConfiguration<Convidados>
{
    public void Configure(EntityTypeBuilder<Convidados> builder)
    {
        builder.ToTable("Convidados");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IdEvento)
            .IsRequired();

        builder.Property(c => c.IdFoto)
            .IsRequired();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        // Relacionamentos
        builder.HasOne(c => c.Evento)
            .WithMany(e => e.Convidados)
            .HasForeignKey(c => c.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Foto)
            .WithMany(f => f.Convidados)
            .HasForeignKey(c => c.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Contribuicoes)
            .WithOne(con => con.Convidado)
            .HasForeignKey(con => con.IdConvidado)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.EnviosConvite)
            .WithOne(ec => ec.Convidado)
            .HasForeignKey(ec => ec.IdConvidado)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Acompanhantes)
            .WithOne(a => a.Convidado)
            .HasForeignKey(a => a.IdConvite)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
