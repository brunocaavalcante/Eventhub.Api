namespace Eventhub.Domain.Entities;

public class EventoFoto
{
    public int Id { get; set; }
    public int IdFoto { get; set; }
    public int Ordem { get; set; }

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public Evento Evento { get; set; } = null!;
}
