namespace Eventhub.Domain.Entities;

public class Convidados
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdFoto { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Fotos Foto { get; set; } = null!;
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
    public ICollection<EnvioConvite> EnviosConvite { get; set; } = new List<EnvioConvite>();
    public ICollection<Acompanhantes> Acompanhantes { get; set; } = new List<Acompanhantes>();
}
