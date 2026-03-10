namespace Eventhub.Application.DTOs;

public class ConfiguracaoVisibilidadeDto
{
    public bool GaleriaFotos { get; set; } = true;
    public bool ChatConvidados { get; set; } = true;
    public bool ListaPresentes { get; set; } = true;
    public bool ListaConvidados { get; set; } = true;
    public bool AgendaEvento { get; set; } = true;
}
