namespace Eventhub.Domain.Entities;

public class TipoEvento
{
    public int Id { get; set; }
    public int IdFoto { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
