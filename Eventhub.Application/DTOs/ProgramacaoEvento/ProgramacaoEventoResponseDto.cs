namespace Eventhub.Application.DTOs;

public class ProgramacaoEventoResponseDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public string NomeEvento { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan Duracao { get; set; }
    public string Local { get; set; } = string.Empty;
    public string Responsavel { get; set; } = string.Empty;
    public int IdStatus { get; set; }
    public string DescricaoStatus { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
}
