namespace Eventhub.Application.DTOs;

public class ConviteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nome2 { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public int IdFoto { get; set; }
}

public class EnvioConviteDto
{
    public int Id { get; set; }
    public int IdConvite { get; set; }
    public int IdParticipante { get; set; }
    public int IdEvento { get; set; }
    public DateTime DataEnvio { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MensagemResposta { get; set; }
    public int QtdAcompanhantes { get; set; }
}