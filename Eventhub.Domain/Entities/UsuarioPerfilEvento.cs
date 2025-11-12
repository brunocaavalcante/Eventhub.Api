namespace Eventhub.Domain.Entities;

public class UsuarioPerfilEvento
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdUsuario { get; set; }
    public int IdPerfil { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public Perfil Perfil { get; set; } = null!;
}
