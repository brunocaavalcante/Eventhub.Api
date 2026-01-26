namespace Eventhub.Application.DTOs;

public class PresenteDetalhesDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Valor { get; set; }
    public string? LinkProduto { get; set; }
    public StatusPresenteDto Status { get; set; } = null!;
    public CategoriaPresenteDto Categoria { get; set; } = null!;
    public List<ContribuicaoDetalhesDto> Contribuicoes { get; set; } = new();
    public List<FotoDto>? Imagens { get; set; }
}
