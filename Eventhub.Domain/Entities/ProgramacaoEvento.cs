namespace Eventhub.Domain.Entities;

public class ProgramacaoEvento
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public TimeSpan Duracao { get; set; }
    public string Local { get; set; } = string.Empty;
    public int IdFoto { get; set; }
    public DateTime DataCadastro { get; set; }
    public string Responsavel { get; set; } = string.Empty;

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Fotos Foto { get; set; } = null!;
    public ICollection<ResponsavelProgramacao> Responsaveis { get; set; } = new List<ResponsavelProgramacao>();
    public int IdStatus { get; set; }
    public StatusProgramacao Status { get; set; } = null!;
}
