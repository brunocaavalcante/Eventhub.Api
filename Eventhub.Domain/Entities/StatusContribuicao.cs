namespace Eventhub.Domain.Entities;

public class StatusContribuicao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
}
