namespace Eventhub.Application.DTOs;

public class UpdatePresenteDto
{
    public int Id { get; set; }
    public int IdCategoriaPresente { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string LinkProduto { get; set; } = string.Empty;
    public ICollection<UpdateFotoDto>? Imagens { get; set; }
}
