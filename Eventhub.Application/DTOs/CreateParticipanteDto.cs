namespace Eventhub.Application.DTOs;

public class CreateParticipanteDto
{
    public int IdEvento { get; set; }
    public int IdPerfil { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
}
