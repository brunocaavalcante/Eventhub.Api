namespace Eventhub.Application.DTOs;

public class PermissoesPerfilDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public List<PermissaoDto> Permissoes { get; set; } = new List<PermissaoDto>();
}

public class PermissaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Chave { get; set; } = string.Empty;
    public ModuloDto Modulo { get; set; } = null!;
}

public class ModuloDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;
    public string Rota { get; set; } = string.Empty;
    public int Ordem { get; set; }
}
