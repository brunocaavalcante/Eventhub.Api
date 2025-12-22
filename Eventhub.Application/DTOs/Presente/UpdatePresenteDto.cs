namespace Eventhub.Application.DTOs;

public class UpdatePresenteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public ICollection<UpdateFotoDto>? Imagens { get; set; }
}
