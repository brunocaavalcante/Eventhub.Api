namespace Eventhub.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string KeycloakId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Foto { get; set; }
    public string? Telefone { get; set; }
    public DateTime DataCadastro { get; set; }
    public string Status { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    public ICollection<UsuarioPerfilEvento> UsuarioPerfis { get; set; } = new List<UsuarioPerfilEvento>();
    public ICollection<ResponsavelProgramacao> ResponsaveisProgramacao { get; set; } = new List<ResponsavelProgramacao>();
    public ICollection<Notificacao> NotificacoesEnviadas { get; set; } = new List<Notificacao>();
    public ICollection<Notificacao> NotificacoesRecebidas { get; set; } = new List<Notificacao>();
    public ICollection<ComentarioFoto> Comentarios { get; set; } = new List<ComentarioFoto>();
    public ICollection<CurtidaFoto> Curtidas { get; set; } = new List<CurtidaFoto>();
    public ICollection<UsuarioPermissao> UsuarioPermissoes { get; set; } = new List<UsuarioPermissao>();
}
