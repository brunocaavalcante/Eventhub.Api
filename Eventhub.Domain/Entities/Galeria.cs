namespace Eventhub.Domain.Entities;

public class Galeria
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdFoto { get; set; }
    public int Ordem { get; set; }
    public string Visibilidade { get; set; } = string.Empty;
    public string Legenda { get; set; } = string.Empty;
    public DateTime Data { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Fotos Foto { get; set; } = null!;
}
