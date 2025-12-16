namespace Eventhub.Domain.Entities;

public class Acompanhantes
{
    public int Id { get; set; }
    public int IdParticipante { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Relacionamentos
    public Participante Participante { get; set; } = null!;
}
