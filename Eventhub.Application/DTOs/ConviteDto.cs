namespace Eventhub.Application.DTOs;

public class ConviteDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int Opacity { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nome2 { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public FotoDto? Foto { get; set; }
}

public class CreateConviteDto
{
    public int IdEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nome2 { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public UploadFotoDto Foto { get; set; }
}

public class UpdateConviteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Nome2 { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public string TemaConvite { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public UpdateFotoDto? Foto { get; set; }
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