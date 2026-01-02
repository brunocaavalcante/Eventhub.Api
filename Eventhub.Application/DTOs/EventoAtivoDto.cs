namespace Eventhub.Application.DTOs;

public class EventoAtivoDto
{
    //teste
    public int Id { get; set; }
    public int IdStatus { get; set; }
    public string Status { get; set; } = string.Empty;
    public int IdTipoEvento { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int MaxConvidado { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string? FotoCapaBase64 { get; set; }
    public string TipoData
    {
        get
        {
            return (DataInicio.Date == DataFim.Date)
                    ? "unica"
                    : "periodo";
        }
    }
}