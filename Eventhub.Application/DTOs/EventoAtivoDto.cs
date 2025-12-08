namespace Eventhub.Application.DTOs;

public class EventoAtivoDto
{
    public int Id { get; set; }
    public int IdStatus { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int MaxConvidado { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string? FotoCapaBase64 { get; set; }
}