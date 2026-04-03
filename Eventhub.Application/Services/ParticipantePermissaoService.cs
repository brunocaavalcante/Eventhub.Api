using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class ParticipantePermissaoService : IParticipantePermissaoService
{
    private readonly IParticipantePermissaoRepository _participantePermissaoRepository;
    private readonly IPermissaoRepository _permissaoRepository;

    public ParticipantePermissaoService(
        IParticipantePermissaoRepository participantePermissaoRepository,
        IPermissaoRepository permissaoRepository)
    {
        _participantePermissaoRepository = participantePermissaoRepository;
        _permissaoRepository = permissaoRepository;
    }

    public async Task ConfigurarPermissoesParticipanteAsync(int idParticipante, Dictionary<string, bool> permissoes)
    {
        foreach (var (chave, concedida) in permissoes)
        {
            var permissao = await _permissaoRepository.GetByChaveAsync(chave);
            if (permissao == null)
                continue; // Permissão não existe, pular

            // Verificar se já existe
            var existente = await _participantePermissaoRepository
                .GetByParticipantePermissaoAsync(idParticipante, permissao.Id);

            if (existente != null)
            {
                // Atualizar
                existente.Concedida = concedida;
                _participantePermissaoRepository.Update(existente);
            }
            else
            {
                // Criar novo
                var novaPermissao = new ParticipantePermissao
                {
                    IdParticipante = idParticipante,
                    IdPermissao = permissao.Id,
                    Concedida = concedida
                };
                await _participantePermissaoRepository.AddAsync(novaPermissao);
            }
        }
    }

    public async Task<Dictionary<string, bool>> ObterPermissoesParticipanteAsync(int idParticipante)
    {
        var permissoesParticipante = await _participantePermissaoRepository
            .GetByParticipanteAsync(idParticipante);

        var resultado = new Dictionary<string, bool>();

        foreach (var permissaoParticipante in permissoesParticipante)
        {
            var permissao = await _permissaoRepository.GetByIdAsync(permissaoParticipante.IdPermissao);
            if (permissao != null)
            {
                resultado[permissao.Chave] = permissaoParticipante.Concedida;
            }
        }

        return resultado;
    }

    public async Task RemoverPermissoesParticipanteAsync(int idParticipante)
    {
        var permissoes = await _participantePermissaoRepository
            .GetByParticipanteAsync(idParticipante);

        foreach (var permissao in permissoes)
        {
            _participantePermissaoRepository.Remove(permissao);
        }
    }
}
