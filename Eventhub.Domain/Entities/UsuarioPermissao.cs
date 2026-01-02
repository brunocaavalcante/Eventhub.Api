namespace Eventhub.Domain.Entities;

public class UsuarioPermissao
{
    public int Id { get; set; }
    public int IdUsuario { get; set; }
    public int IdPermissao { get; set; }
    public bool Concedida { get; set; }

    // Relacionamentos
    public Usuario Usuario { get; set; } = null!;
    public Permissao Permissao { get; set; } = null!;
}
