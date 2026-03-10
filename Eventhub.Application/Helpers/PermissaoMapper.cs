using Eventhub.Application.DTOs;

namespace Eventhub.Application.Helpers;

public static class PermissaoMapper
{
    private static readonly Dictionary<string, string> ChaveParaPropriedade = new()
    {
        { "galeria.view", nameof(ConfiguracaoVisibilidadeDto.GaleriaFotos) },
        { "chat.view", nameof(ConfiguracaoVisibilidadeDto.ChatConvidados) },
        { "presente.view", nameof(ConfiguracaoVisibilidadeDto.ListaPresentes) },
        { "convidados.view", nameof(ConfiguracaoVisibilidadeDto.ListaConvidados) },
        { "agenda.view", nameof(ConfiguracaoVisibilidadeDto.AgendaEvento) }
    };

    public static Dictionary<string, bool> DtoToPermissoes(ConfiguracaoVisibilidadeDto dto)
    {
        return new Dictionary<string, bool>
        {
            { "galeria.view", dto.GaleriaFotos },
            { "chat.view", dto.ChatConvidados },
            { "presente.view", dto.ListaPresentes },
            { "convidados.view", dto.ListaConvidados },
            { "agenda.view", dto.AgendaEvento }
        };
    }

    public static ConfiguracaoVisibilidadeDto PermissoesToDto(Dictionary<string, bool> permissoes)
    {
        return new ConfiguracaoVisibilidadeDto
        {
            GaleriaFotos = permissoes.GetValueOrDefault("galeria.view", true),
            ChatConvidados = permissoes.GetValueOrDefault("chat.view", true),
            ListaPresentes = permissoes.GetValueOrDefault("presente.view", true),
            ListaConvidados = permissoes.GetValueOrDefault("convidados.view", true),
            AgendaEvento = permissoes.GetValueOrDefault("agenda.view", true)
        };
    }
}
