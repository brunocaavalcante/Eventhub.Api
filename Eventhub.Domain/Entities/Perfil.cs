namespace Eventhub.Domain.Entities;

public class Perfil
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<UsuarioPerfilEvento> UsuarioPerfis { get; set; } = new List<UsuarioPerfilEvento>();
}
