namespace Eventhub.Application.DTOs;

public class ContribuicaoPresenteDto
{
    public int Id { get; set; }
    public int IdPresente { get; set; }
    public int IdParticipante { get; set; }
    public int IdStatusContribuicao { get; set; }
    public decimal Valor { get; set; }
    public string FormaPagamento { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }

    public string? Justificativa { get; set; }
}
