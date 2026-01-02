namespace Eventhub.Domain.Entities;

public class Presente
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string LinkProduto { get; set; } = string.Empty;
    public int IdCategoria { get; set; }
    public int IdStatus { get; set; }
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public StatusPresente Status { get; set; } = null!;
    public CategoriaPresente Categoria { get; set; } = null!;
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
    public ICollection<Galeria> Galerias { get; set; } = new List<Galeria>(); 
}
