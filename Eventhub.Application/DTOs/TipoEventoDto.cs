namespace Eventhub.Application.DTOs;

public class TipoEventoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdFoto { get; set; }
}
