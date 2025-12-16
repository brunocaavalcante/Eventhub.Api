namespace Eventhub.Application.DTOs;

public class PerfilDto
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public char Status { get; set; }
}
