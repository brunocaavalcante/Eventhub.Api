public class UpdateContribuicaoPresenteDto
{
    public int Id { get; set; }
    public decimal Valor { get; set; }
    public string? Justificativa { get; set; }
    public StatusContribuicaoDto Status { get; set; }
}

