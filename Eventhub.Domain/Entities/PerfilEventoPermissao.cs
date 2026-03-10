namespace Eventhub.Domain.Entities;

public class PerfilEventoPermissao
{
    public int Id { get; set; }
    public int IdPerfil { get; set; }
    public int IdEvento { get; set; }
    public int IdPermissao { get; set; }
    public bool Concedida { get; set; }

    // Relacionamentos
    public Perfil Perfil { get; set; } = null!;
    public Evento Evento { get; set; } = null!;
    public Permissao Permissao { get; set; } = null!;
}
