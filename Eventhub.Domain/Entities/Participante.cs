namespace Eventhub.Domain.Entities;

public class Participante
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdUsuario { get; set; }
    public int IdPerfil { get; set; }
    public bool CadastroPendente { get; set; }
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public Perfil Perfil { get; set; } = null!;
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
    public ICollection<EnvioConvite> EnviosConvite { get; set; } = new List<EnvioConvite>();
    public ICollection<Acompanhantes> Acompanhantes { get; set; } = new List<Acompanhantes>();
}
