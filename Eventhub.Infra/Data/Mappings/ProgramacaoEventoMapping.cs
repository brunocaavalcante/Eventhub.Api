using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ProgramacaoEventoMapping : IEntityTypeConfiguration<ProgramacaoEvento>
{
    public void Configure(EntityTypeBuilder<ProgramacaoEvento> builder)
    {
        builder.ToTable("ProgramacaoEvento");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.IdEvento)
            .IsRequired();

        builder.Property(p => p.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Data)
            .IsRequired();

        builder.Property(p => p.Duracao)
            .IsRequired();

        builder.Property(p => p.Local)
            .HasMaxLength(200);

        builder.Property(p => p.IdFoto)
            .IsRequired();

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        builder.Property(p => p.Responsavel)
            .HasMaxLength(200);

        // Relacionamentos
        builder.HasOne(p => p.Evento)
            .WithMany(e => e.Programacoes)
            .HasForeignKey(p => p.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Foto)
            .WithMany(f => f.Programacoes)
            .HasForeignKey(p => p.IdFoto)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Responsaveis)
            .WithOne(r => r.Programacao)
            .HasForeignKey(r => r.IdProgramacao)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Status)
            .WithMany(s => s.Programacoes)
            .HasForeignKey(p => p.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
