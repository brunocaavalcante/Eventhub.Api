namespace Eventhub.Domain.Entities;

public class Perfil
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public char Status { get; set; } = 'A';

    // Relacionamentos
    public ICollection<UsuarioPerfilEvento> UsuarioPerfis { get; set; } = new List<UsuarioPerfilEvento>();
    public ICollection<PerfilPermissao> PerfilPermissoes { get; set; } = new List<PerfilPermissao>();
    public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
}
