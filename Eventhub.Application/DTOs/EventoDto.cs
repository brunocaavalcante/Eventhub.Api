namespace Eventhub.Application.DTOs;

public class EventoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdTipoEvento { get; set; }
    public int IdStatus { get; set; }
    public int IdEndereco { get; set; }
    public int IdUsuarioCriador { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime DataFim { get; set; }
    public int MaxConvidado { get; set; }
}
