namespace Eventhub.Domain.Entities;

public class Participante
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdUsuario { get; set; }
    public int IdPerfil { get; set; }
    public bool CadastroPendente { get; set; }
    public DateTime DataCadastro { get; set; }
    public int? IdStatusConvite { get; set; }
    public int QtdAcompanhantes { get; set; }
    public string? MensagemOrganizador { get; set; }
    public string? MotivoRecusa { get; set; }
    public DateTime? DataResposta { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public Perfil Perfil { get; set; } = null!;
    public StatusEnvioConvite? StatusConvite { get; set; }
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
    public ICollection<Acompanhantes> Acompanhantes { get; set; } = new List<Acompanhantes>();
}
