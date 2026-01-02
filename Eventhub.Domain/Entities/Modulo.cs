namespace Eventhub.Domain.Entities;

public class Modulo
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string Rota { get; set; } = string.Empty;
    public int Ordem { get; set; }

    // Relacionamentos
    public ICollection<Permissao> Permissoes { get; set; } = new List<Permissao>();
}
