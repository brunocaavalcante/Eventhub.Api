namespace Eventhub.Domain.Entities;

public class ComentarioFoto
{
    public int Id { get; set; }
    public int IdFoto { get; set; }
    public int IdUsuario { get; set; }
    public DateTime Data { get; set; }
    public string Comentario { get; set; } = string.Empty;

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
