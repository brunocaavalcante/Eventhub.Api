namespace Eventhub.Application.DTOs;

public class PresenteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string? Imagem { get; set; }
    public bool Ativo { get; set; }
}