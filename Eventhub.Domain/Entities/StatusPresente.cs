namespace Eventhub.Domain.Entities;

public class StatusPresente
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<Presente> Presentes { get; set; } = new List<Presente>();
}
