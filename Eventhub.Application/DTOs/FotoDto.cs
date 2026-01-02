namespace Eventhub.Application.DTOs;

public class FotoDto
{
    public int Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public DateTime DataUpload { get; set; }
    public int TamanhoKB { get; set; }
    public string Base64 { get; set; } = string.Empty;
}

public class UploadFotoDto
{
    public string NomeArquivo { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
    public string TipoImagem { get; set; } = string.Empty;
}

public class UpdateFotoDto
{
    public int Id { get; set; }
    public string NomeArquivo { get; set; } = string.Empty;
    public string Base64 { get; set; } = string.Empty;
}
