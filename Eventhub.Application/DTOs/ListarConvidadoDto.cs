namespace Eventhub.Application.DTOs;

public class ListarConvidadoDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public int QuandidadeAcompanhantes { get; set; } = 0;
    public string Foto { get; set; } = string.Empty;
    public string StatusConfirmacao { get; set; } = string.Empty;
}
