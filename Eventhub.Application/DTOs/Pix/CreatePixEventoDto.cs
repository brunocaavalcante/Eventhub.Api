using Eventhub.Domain.Enums;

namespace Eventhub.Application.DTOs;

public class CreatePixEventoDto
{
    public int IdEvento { get; set; }
    public FinalidadePix Finalidade { get; set; }
    public string NomeBeneficiario { get; set; } = string.Empty;
    public string QRCodePix { get; set; } = string.Empty;
}
