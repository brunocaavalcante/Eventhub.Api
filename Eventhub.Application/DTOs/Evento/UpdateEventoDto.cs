namespace Eventhub.Application.DTOs.Evento;

public class UpdateEventoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdTipoEvento { get; set; }
    public int MaxConvidado { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public EnderecoEventoDto Endereco { get; set; } = new EnderecoEventoDto();
    public List<UploadFotoDto> Imagens { get; set; } = new();
}
