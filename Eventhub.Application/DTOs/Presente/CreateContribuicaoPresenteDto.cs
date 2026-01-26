namespace Eventhub.Application.DTOs;

public class CreateContribuicaoPresenteDto
{
    public int IdPresente { get; set; }
    public int IdParticipante { get; set; }
    public decimal Valor { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public UploadFotoDto Comprovante { get; set; } = new();
}
