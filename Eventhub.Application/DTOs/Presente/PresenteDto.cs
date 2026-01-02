namespace Eventhub.Application.DTOs;

public class PresenteDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string LinkProduto { get; set; } = string.Empty;
    public CategoriaPresenteDto Categoria { get; set; }
    public StatusPresenteDto Status { get; set; }
    public List<FotoDto> Imagens { get; set; } = new();
}