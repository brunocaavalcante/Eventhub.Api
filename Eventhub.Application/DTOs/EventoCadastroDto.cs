namespace Eventhub.Application.DTOs;

public class EventoCadastroDto
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdTipoEvento { get; set; }
    public int IdUsuarioCriador { get; set; }
    public int MaxConvidado { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public EnderecoEventoDto Endereco { get; set; } = new EnderecoEventoDto();
    public List<UploadFotoDto> Imagens { get; set; } = new();
    public List<CreateParticipanteDto> Participantes { get; set; } = new();
}