namespace Eventhub.Application.DTOs;

public class ContribuicaoDetalhesDto
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataCadastro { get; set; }
    public StatusContribuicaoDto Status { get; set; }
    public string? Justificativa { get; set; }
    public FotoDto? Comprovante { get; set; }
    public ParticipanteContribuicaoDto Participante { get; set; } = null!;
}
