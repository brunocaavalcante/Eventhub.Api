namespace Eventhub.Application.DTOs;

public class ParticipanteDto
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public PerfilDto? Perfil { get; set; }
    public UsuarioInfoDto? Usuario { get; set; }
    public bool CadastroPendente { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? IdStatusConvite { get; set; }
    public string? StatusConvite { get; set; }
    public int QtdAcompanhantes { get; set; }
    public string? MensagemOrganizador { get; set; }
    public string? MotivoRecusa { get; set; }
    public DateTime? DataResposta { get; set; }
}
