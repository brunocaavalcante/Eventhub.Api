using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class EventoMapping : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("Evento");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.IdTipoEvento)
            .IsRequired();

        builder.Property(e => e.IdStatus)
            .IsRequired();

        builder.Property(e => e.IdEndereco)
            .IsRequired();

        builder.Property(e => e.CpfInclusao)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.DataInicio)
            .IsRequired();

        builder.Property(e => e.DataInclusao)
            .IsRequired();

        builder.Property(e => e.DataFim)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.MaxConvidado)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(e => e.TipoEvento)
            .WithMany(t => t.Eventos)
            .HasForeignKey(e => e.IdTipoEvento)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Status)
            .WithMany(s => s.Eventos)
            .HasForeignKey(e => e.IdStatus)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Endereco)
            .WithMany(en => en.Eventos)
            .HasForeignKey(e => e.IdEndereco)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UsuarioCriador)
            .WithMany(u => u.Eventos)
            .HasForeignKey(e => e.CpfInclusao)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.UsuarioPerfis)
            .WithOne(up => up.Evento)
            .HasForeignKey(up => up.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EventoFotos)
            .WithOne(ef => ef.Evento)
            .HasForeignKey(ef => ef.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Galerias)
            .WithOne(g => g.Evento)
            .HasForeignKey(g => g.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Programacoes)
            .WithOne(p => p.Evento)
            .HasForeignKey(p => p.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Notificacoes)
            .WithOne(n => n.Evento)
            .HasForeignKey(n => n.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Convidados)
            .WithOne(c => c.Evento)
            .HasForeignKey(c => c.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.EnviosConvite)
            .WithOne(ec => ec.Evento)
            .HasForeignKey(ec => ec.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
