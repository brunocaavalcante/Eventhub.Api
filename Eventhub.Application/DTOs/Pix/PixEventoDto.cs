using Eventhub.Domain.Enums;

namespace Eventhub.Application.DTOs;

public class PixEventoDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public FinalidadePix Finalidade { get; set; }
    public string FinalidadeDescricao { get; set; } = string.Empty;
    public string NomeBeneficiario { get; set; } = string.Empty;
    public string QRCodePix { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
