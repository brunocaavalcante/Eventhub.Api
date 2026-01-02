namespace Eventhub.Domain.Entities;

public class Convite
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int Opacity { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Nome2 { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public int IdFoto { get; set; }

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public Evento Evento { get; set; } = null!;
    public ICollection<EnvioConvite> EnviosConvite { get; set; } = new List<EnvioConvite>();
}
