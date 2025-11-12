namespace Eventhub.Domain.Entities;

public class Notificacao
{
    public int Id { get; set; }
    public int IdEvento { get; set; }
    public int IdUsuarioOrigem { get; set; }
    public int IdUsuarioDestino { get; set; }
    public DateTime Data { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string LinkAcao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Prioridade { get; set; }
    public DateTime DataCadastro { get; set; }
    public DateTime DataEnvio { get; set; }
    public DateTime DataLeitura { get; set; }

    // Relacionamentos
    public Evento Evento { get; set; } = null!;
    public Usuario UsuarioOrigem { get; set; } = null!;
    public Usuario UsuarioDestino { get; set; } = null!;
}
