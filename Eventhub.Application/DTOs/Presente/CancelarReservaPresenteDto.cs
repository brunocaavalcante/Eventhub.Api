namespace Eventhub.Application.DTOs;

public class CancelarReservaPresenteDto
{
    public int IdParticipante { get; set; }
    public string Justificativa { get; set; } = string.Empty;
}
