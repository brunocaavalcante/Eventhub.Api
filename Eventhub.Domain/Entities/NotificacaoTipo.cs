namespace Eventhub.Domain.Entities;

public class NotificacaoTipo
{
    public int Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string TextoPadrao { get; set; } = string.Empty;
    public string IconePadrao { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<Notificacao> Notificacoes { get; set; } = new List<Notificacao>();
}
