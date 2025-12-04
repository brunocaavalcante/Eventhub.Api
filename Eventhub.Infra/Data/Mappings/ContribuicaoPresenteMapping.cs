using Eventhub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventhub.Infra.Data.Mappings;

public class ContribuicaoPresenteMapping : IEntityTypeConfiguration<ContribuicaoPresente>
{
    public void Configure(EntityTypeBuilder<ContribuicaoPresente> builder)
    {
        builder.ToTable("ContribuicaoPresente");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.IdPresente)
            .IsRequired();

        builder.Property(c => c.IdParticipante)
            .IsRequired();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Descricao)
            .HasMaxLength(1000);

        builder.Property(c => c.Valor)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.LinkProduto)
            .HasMaxLength(500);

        builder.Property(c => c.FormaPagamento)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(c => c.Presente)
            .WithMany(p => p.Contribuicoes)
            .HasForeignKey(c => c.IdPresente)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Participante)
            .WithMany(participante => participante.Contribuicoes)
            .HasForeignKey(c => c.IdParticipante)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
