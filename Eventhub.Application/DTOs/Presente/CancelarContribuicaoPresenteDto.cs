namespace Eventhub.Application.DTOs;

public class CancelarContribuicaoPresenteDto
{
    public int IdContribuicao { get; set; }
    public string Justificativa { get; set; } = string.Empty;
}
