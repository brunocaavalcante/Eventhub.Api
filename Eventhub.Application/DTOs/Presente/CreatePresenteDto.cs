namespace Eventhub.Application.DTOs;

public class CreatePresenteDto
{
    public int IdEvento { get; set; }
    public int IdCategoria { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public List<UploadFotoDto> Imagens { get; set; } = new();
}
