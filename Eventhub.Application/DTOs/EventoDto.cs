namespace Eventhub.Application.DTOs;

public class EventoDto
{
    public int Id { get; set; }
    public int IdTipoEvento { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataInclusao { get; set; }
    public DateTime DataFim { get; set; }
    public int MaxConvidado { get; set; }
    public EnderecoEventoDto Endereco { get; set; } = null!;

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
