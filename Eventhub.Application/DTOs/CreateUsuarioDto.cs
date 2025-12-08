namespace Eventhub.Application.DTOs;

public class CreateUsuarioDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public int? IdFoto { get; set; }
}
