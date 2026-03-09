namespace Eventhub.Application.DTOs;

public class ConfirmarPresencaDto
{
    public Guid TokenEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int QtdAcompanhantes { get; set; }
    public string? MensagemOrganizador { get; set; }
}
