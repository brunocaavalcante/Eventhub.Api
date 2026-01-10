using Eventhub.Domain.Enums;

namespace Eventhub.Application.DTOs;

public class UpdatePixEventoDto
{
    public int Id { get; set; }
    public string NomeBeneficiario { get; set; } = string.Empty;
    public string QRCodePix { get; set; } = string.Empty;
}
