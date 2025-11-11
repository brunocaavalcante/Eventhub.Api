namespace Eventhub.Domain.Entities;

public class CurtidaFoto
{
    public int Id { get; set; }
    public int IdFoto { get; set; }
    public int IdUsuario { get; set; }
    public DateTime Data { get; set; }

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
