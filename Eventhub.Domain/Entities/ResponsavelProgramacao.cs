namespace Eventhub.Domain.Entities;

public class ResponsavelProgramacao
{
    public int Id { get; set; }
    public int IdProgramacao { get; set; }
    public int IdUsuario { get; set; }
    public string Funcao { get; set; } = string.Empty;

    // Relacionamentos
    public ProgramacaoEvento Programacao { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
