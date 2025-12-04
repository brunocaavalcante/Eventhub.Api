namespace Eventhub.Application.DTOs;

public class EventoCadastroDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdTipoEvento { get; set; }
    public int IdUsuarioCriador { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int MaxConvidado { get; set; }
    public EnderecoEventoDto Endereco { get; set; } = new EnderecoEventoDto();
    public List<ImagemEventoDto> Imagens { get; set; } = new();
}

public class ImagemEventoDto
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
    public int TipoImagem { get; set; }
}
