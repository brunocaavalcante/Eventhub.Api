namespace Eventhub.Application.DTOs;

public class RecusarConviteDto
{
    public Guid TokenEvento { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? MotivoRecusa { get; set; }
}