namespace Eventhub.Application.Helpers;

public static class EmailTemplates
{
    private static readonly string TemplatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "Email");

    private static string CarregarTemplate(string nomeArquivo)
    {
        var caminhoCompleto = Path.Combine(TemplatesPath, nomeArquivo);
        
        if (!File.Exists(caminhoCompleto))
            throw new FileNotFoundException($"Template de email não encontrado: {caminhoCompleto}");
        
        return File.ReadAllText(caminhoCompleto);
    }

    public static string GetEventoCanceladoTemplate(
        string nomeDestinatario, 
        string nomeEvento, 
        string justificativa)
    {
        var template = CarregarTemplate("EventoCancelado.html");
        
        return template
            .Replace("{{NomeDestinatario}}", nomeDestinatario)
            .Replace("{{NomeEvento}}", nomeEvento)
            .Replace("{{Justificativa}}", justificativa);
    }

    public static string GetEventoReativadoTemplate(
        string nomeDestinatario, 
        string nomeEvento)
    {
        var template = CarregarTemplate("EventoReativado.html");
        
        return template
            .Replace("{{NomeDestinatario}}", nomeDestinatario)
            .Replace("{{NomeEvento}}", nomeEvento);
    }

    public static string GetContribuicaoCanceladaTemplate(
        string nomeDestinatario, 
        string nomePresente, 
        string nomeEvento, 
        decimal valorContribuicao, 
        string justificativa)
    {
        var valorFormatado = valorContribuicao.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
        var template = CarregarTemplate("ContribuicaoCancelada.html");
        
        return template
            .Replace("{{NomeDestinatario}}", nomeDestinatario)
            .Replace("{{NomePresente}}", nomePresente)
            .Replace("{{NomeEvento}}", nomeEvento)
            .Replace("{{ValorContribuicao}}", valorFormatado)
            .Replace("{{Justificativa}}", justificativa);
    }
}
