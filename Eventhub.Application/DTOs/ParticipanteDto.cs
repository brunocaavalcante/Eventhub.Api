namespace Eventhub.Application.DTOs;

public class ParticipanteDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public PerfilDto? Perfil { get; set; }
    public UsuarioInfoDto Usuario { get; set; } = new();
    public bool CadastroPendente { get; set; }
    public string Status { get; set; } = string.Empty;
}
