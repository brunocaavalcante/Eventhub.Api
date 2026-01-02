namespace Eventhub.Domain.Entities;

public class EnvioConvite
{
    public int Id { get; set; }
    public int IdConvite { get; set; }
    public int IdParticipante { get; set; }
    public int IdEvento { get; set; }
    public DateTime DataEnvio { get; set; }
    public int? IdStatusEnvioConvite { get; set; }
    public string MensagemResposta { get; set; } = string.Empty;
    public int QtdAcompanhantes { get; set; }

    // Relacionamentos
    public Convite Convite { get; set; } = null!;
    public Participante Participante { get; set; } = null!;
    public Evento Evento { get; set; } = null!;
    public StatusEnvioConvite? StatusEnvioConvite { get; set; }
}
