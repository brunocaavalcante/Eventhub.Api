namespace Eventhub.Domain.Entities;

public class StatusProgramacao
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<ProgramacaoEvento> Programacoes { get; set; } = new List<ProgramacaoEvento>();
}
