using Eventhub.Domain.Enums;

namespace Eventhub.Application.DTOs;

public class NotificacaoCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int IdUsuarioDestino { get; set; }
    public int IdEvento { get; set; }
    public int IdUsuarioOrigem { get; set; }
    public int? IdTipoNotificacao { get; set; }
    public EnumNotificacaoStatus Status { get; set; } = EnumNotificacaoStatus.Enviada;
    public EnumNotificacaoPrioridade Prioridade { get; set; } = EnumNotificacaoPrioridade.Media;
    public string LinkAcao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
}
