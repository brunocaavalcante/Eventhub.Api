namespace Eventhub.Domain.Entities;

public class Fotos
{
    public int Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public int TamanhoKB { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? PublicId { get; set; }
    public string ContentType { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<TipoEvento> TipoEventos { get; set; } = new List<TipoEvento>();
    public ICollection<Galeria> Galerias { get; set; } = new List<Galeria>();
    public ICollection<ProgramacaoEvento> Programacoes { get; set; } = new List<ProgramacaoEvento>();
    public ICollection<ComentarioFoto> Comentarios { get; set; } = new List<ComentarioFoto>();
    public ICollection<CurtidaFoto> Curtidas { get; set; } = new List<CurtidaFoto>();
    public ICollection<ContribuicaoPresente> Contribuicoes { get; set; } = new List<ContribuicaoPresente>();
}
