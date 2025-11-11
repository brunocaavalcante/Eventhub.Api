namespace Eventhub.Domain.Entities;

public class Acompanhantes
{
    public int Id { get; set; }
    public int IdConvite { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Relacionamentos
    public Convidados Convidado { get; set; } = null!;
}
