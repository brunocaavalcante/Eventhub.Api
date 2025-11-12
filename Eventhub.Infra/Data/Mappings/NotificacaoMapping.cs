using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class NotificacaoMapping : IEntityTypeConfiguration<Notificacao>
{
    public void Configure(EntityTypeBuilder<Notificacao> builder)
    {
        builder.ToTable("Notificacao");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.IdEvento)
            .IsRequired();

        builder.Property(n => n.IdUsuarioOrigem)
            .IsRequired();

        builder.Property(n => n.IdUsuarioDestino)
            .IsRequired();

        builder.Property(n => n.Data)
            .IsRequired();

        builder.Property(n => n.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Descricao)
            .HasMaxLength(1000);

        builder.Property(n => n.LinkAcao)
            .HasMaxLength(500);

        builder.Property(n => n.Icone)
            .HasMaxLength(100);

        builder.Property(n => n.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(n => n.Prioridade)
            .IsRequired();

        builder.Property(n => n.DataCadastro)
            .IsRequired();

        builder.Property(n => n.DataEnvio)
            .IsRequired();

        builder.Property(n => n.DataLeitura);

        // Relacionamentos
        builder.HasOne(n => n.Evento)
            .WithMany(e => e.Notificacoes)
            .HasForeignKey(n => n.IdEvento)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.UsuarioOrigem)
            .WithMany(u => u.NotificacoesEnviadas)
            .HasForeignKey(n => n.IdUsuarioOrigem)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.UsuarioDestino)
            .WithMany(u => u.NotificacoesRecebidas)
            .HasForeignKey(n => n.IdUsuarioDestino)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
