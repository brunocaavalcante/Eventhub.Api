namespace Eventhub.Application.DTOs;

public class ProgramacaoEventoUpdateDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan? Duracao { get; set; }
    public string Local { get; set; } = string.Empty;
    public int? IdFoto { get; set; }
    public string Responsavel { get; set; } = string.Empty;
    public int IdStatus { get; set; }
}
