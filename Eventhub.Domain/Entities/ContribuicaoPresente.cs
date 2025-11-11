namespace Eventhub.Domain.Entities;

public class ContribuicaoPresente
{
    public int Id { get; set; }
    public int IdPresente { get; set; }
    public int IdConvidado { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string LinkProduto { get; set; } = string.Empty;
    public string FormaPagamento { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Presente Presente { get; set; } = null!;
    public Convidados Convidado { get; set; } = null!;
}
