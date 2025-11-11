namespace Eventhub.Domain.Entities;

public class Evento
{
    public int Id { get; set; }
    public int IdTipoEvento { get; set; }
    public int IdStatus { get; set; }
    public int IdEndereco { get; set; }
    public string CpfInclusao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime DataFim { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int MaxConvidado { get; set; }

    // Relacionamentos
    public TipoEvento TipoEvento { get; set; } = null!;
    public StatusEvento Status { get; set; } = null!;
    public EnderecoEvento Endereco { get; set; } = null!;
    public Usuario UsuarioCriador { get; set; } = null!;
    public ICollection<UsuarioPerfilEvento> UsuarioPerfis { get; set; } = new List<UsuarioPerfilEvento>();
    public ICollection<EventoFoto> EventoFotos { get; set; } = new List<EventoFoto>();
    public ICollection<Galeria> Galerias { get; set; } = new List<Galeria>();
    public ICollection<ProgramacaoEvento> Programacoes { get; set; } = new List<ProgramacaoEvento>();
    public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
    public ICollection<Convidados> Convidados { get; set; } = new List<Convidados>();
    public ICollection<EnvioConvite> EnviosConvite { get; set; } = new List<EnvioConvite>();
}
