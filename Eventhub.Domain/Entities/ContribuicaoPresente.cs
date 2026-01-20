namespace Eventhub.Domain.Entities;

public class ContribuicaoPresente
{
    public int Id { get; set; }
    public int IdPresente { get; set; }
    public int IdParticipante { get; set; }
    public int IdStatusContribuicao { get; set; }
    public int IdFoto { get; set; }
    public decimal Valor { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Presente Presente { get; set; } = null!;
    public Participante Participante { get; set; } = null!;
    public StatusContribuicao StatusContribuicao { get; set; } = null!;
    public Fotos Foto { get; set; } = null!;
}
