using Eventhub.Domain.Enums;

namespace Eventhub.Application.DTOs;

public class NotificacaoResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdUsuarioDestino { get; set; }
    public string NomeUsuarioDestino { get; set; } = string.Empty;
    public int IdUsuarioOrigem { get; set; }
    public string NomeUsuarioOrigem { get; set; } = string.Empty;
    public int IdEvento { get; set; }
    public string NomeEvento { get; set; } = string.Empty;
    public EnumNotificacaoStatus Status { get; set; }
    public EnumNotificacaoPrioridade Prioridade { get; set; }
    public string LinkAcao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public DateTime DataCadastro { get; set; }
    public DateTime DataEnvio { get; set; }
    public DateTime DataLeitura { get; set; }
    public bool Lida => DataLeitura != DateTime.MinValue;
}
