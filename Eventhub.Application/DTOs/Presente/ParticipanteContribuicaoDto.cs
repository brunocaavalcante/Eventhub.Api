namespace Eventhub.Application.DTOs;

public class ParticipanteContribuicaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Foto { get; set; }
}
