namespace Eventhub.Application.DTOs;

public class UsuarioInfoDto
{
    public int Id { get; set; }
    public string KeycloakId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Foto { get; set; }
    public DateTime DataCadastro { get; set; }
    public string Status { get; set; } = string.Empty;
}
