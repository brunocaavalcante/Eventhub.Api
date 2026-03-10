namespace Eventhub.Domain.Entities;

public class ParticipantePermissao
{
    public int Id { get; set; }
    public int IdParticipante { get; set; }
    public int IdPermissao { get; set; }
    public bool Concedida { get; set; }

    // Relacionamentos
    public Participante Participante { get; set; } = null!;
    public Permissao Permissao { get; set; } = null!;
}
