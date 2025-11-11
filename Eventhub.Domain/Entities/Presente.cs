namespace Eventhub.Domain.Entities;

public class Presente
{
    public int Id { get; set; }
    public int IdFoto { get; set; }
    public int IdEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string LinkProduto { get; set; } = string.Empty;
    public int IdStatus { get; set; }
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public Evento Evento { get; set; } = null!;
    public StatusPresente Status { get; set; } = null!;
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
}
