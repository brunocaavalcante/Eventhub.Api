using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PermissaoEventoService : IPermissaoEventoService
{
    private readonly IParticipanteRepository _participanteRepository;
    private readonly IPermissaoRepository _permissaoRepository;
    private readonly IParticipantePermissaoRepository _participantePermissaoRepository;
    private readonly IPerfilEventoPermissaoRepository _perfilEventoPermissaoRepository;

    public PermissaoEventoService(
        IParticipanteRepository participanteRepository,
        IPermissaoRepository permissaoRepository,
        IParticipantePermissaoRepository participantePermissaoRepository,
        IPerfilEventoPermissaoRepository perfilEventoPermissaoRepository)
    {
        _participanteRepository = participanteRepository;
        _permissaoRepository = permissaoRepository;
        _participantePermissaoRepository = participantePermissaoRepository;
        _perfilEventoPermissaoRepository = perfilEventoPermissaoRepository;
    }

    public async Task<bool> TemPermissaoAsync(int idUsuario, int idEvento, string chavePermissao)
    {
        // 1. Obter participante
        var participantes = await _participanteRepository.GetByEventoAsync(idEvento);
        var participante = participantes.FirstOrDefault(p => p.IdUsuario == idUsuario);
        
        if (participante == null)
            return false; // Não é participante do evento

        // 2. Obter permissão
        var permissao = await _permissaoRepository.GetByChaveAsync(chavePermissao);
        if (permissao == null)
            return false; // Permissão não existe

        // 3. Verificar ParticipantePermissao (override individual - maior prioridade)
        var participantePermissao = await _participantePermissaoRepository
            .GetByParticipantePermissaoAsync(participante.Id, permissao.Id);
        
        if (participantePermissao != null)
            return participantePermissao.Concedida;

        // 4. Verificar PerfilEventoPermissao (configuração do evento)
        var perfilEventoPermissao = await _perfilEventoPermissaoRepository
            .GetByPerfilEventoPermissaoAsync(participante.IdPerfil, idEvento, permissao.Id);
        
        if (perfilEventoPermissao != null)
            return perfilEventoPermissao.Concedida;

        // 5. Padrão: tudo liberado (conforme requisito)
        return true;
    }

    public async Task<Dictionary<string, bool>> ListarPermissoesEventoAsync(int idUsuario, int idEvento)
    {
        var permissoesChaves = new[]
        {
            "galeria.view",
            "chat.view",
            "presente.view",
            "convidados.view",
            "agenda.view"
        };

        var resultado = new Dictionary<string, bool>();

        foreach (var chave in permissoesChaves)
        {
            var temPermissao = await TemPermissaoAsync(idUsuario, idEvento, chave);
            resultado[chave] = temPermissao;
        }

        return resultado;
    }
}
