using Eventhub.Domain.Enums;

namespace Eventhub.Domain.Entities;

public class PixEvento
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public FinalidadePix Finalidade { get; set; }
    public string NomeBeneficiario { get; set; } = string.Empty;
    public string QRCodePix { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
}
