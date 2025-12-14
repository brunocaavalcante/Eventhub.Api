namespace Eventhub.Domain.Entities;

public class EnderecoEvento
{
    public int Id { get; set; }
    public string? NomeLocal { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Logradouro { get; set; } = string.Empty;
    public string Cep { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string PontoReferencia { get; set; } = string.Empty;

    // Relacionamentos
    public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
}
