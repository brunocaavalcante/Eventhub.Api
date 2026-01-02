namespace Eventhub.Domain.Entities;

public class PerfilPermissao
{
    public int Id { get; set; }
    public int IdPerfil { get; set; }
    public int IdPermissao { get; set; }

    // Relacionamentos
    public Perfil Perfil { get; set; } = null!;
    public Permissao Permissao { get; set; } = null!;
}
