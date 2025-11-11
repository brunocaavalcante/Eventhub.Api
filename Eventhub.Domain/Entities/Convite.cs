namespace Eventhub.Domain.Entities;

public class Convite
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Nome2 { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public int IdFoto { get; set; }

    // Relacionamentos
    public Fotos Foto { get; set; } = null!;
    public ICollection<EnvioConvite> EnviosConvite { get; set; } = new List<EnvioConvite>();
}
