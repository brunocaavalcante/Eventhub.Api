namespace Eventhub.Domain.Entities;

public class StatusEvento
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
