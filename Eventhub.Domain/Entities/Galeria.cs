namespace Eventhub.Domain.Entities;

using Eventhub.Domain.Enums;

public class Galeria
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int? IdPresente { get; set; }
    public int IdFoto { get; set; }
    public int? Ordem { get; set; }
    public string Visibilidade { get; set; } = string.Empty;
    public string Legenda { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public GaleriaTipo Tipo { get; set; } = GaleriaTipo.Galeria;

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Fotos Foto { get; set; } = null!;
    public Presente? Presente { get; set; }
}
