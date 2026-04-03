using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PerfilEventoPermissaoService : IPerfilEventoPermissaoService
{
    private readonly IPerfilEventoPermissaoRepository _perfilEventoPermissaoRepository;
    private readonly IPermissaoRepository _permissaoRepository;

    public PerfilEventoPermissaoService(
        IPerfilEventoPermissaoRepository perfilEventoPermissaoRepository,
        IPermissaoRepository permissaoRepository)
    {
        _perfilEventoPermissaoRepository = perfilEventoPermissaoRepository;
        _permissaoRepository = permissaoRepository;
    }

    public async Task ConfigurarPermissoesPerfilAsync(int idEvento, int idPerfil, Dictionary<string, bool> permissoes)
    {
        foreach (var (chave, concedida) in permissoes)
        {
            var permissao = await _permissaoRepository.GetByChaveAsync(chave);
            if (permissao == null)
                continue; // Permissão não existe, pular

            // Verificar se já existe
            var existente = await _perfilEventoPermissaoRepository
                .GetByPerfilEventoPermissaoAsync(idPerfil, idEvento, permissao.Id);

            if (existente != null)
            {
                // Atualizar
                existente.Concedida = concedida;
                await _perfilEventoPermissaoRepository.UpdateAsync(existente);
            }
            else
            {
                // Criar novo
                var novaPermissao = new PerfilEventoPermissao
                {
                    IdPerfil = idPerfil,
                    IdEvento = idEvento,
                    IdPermissao = permissao.Id,
                    Concedida = concedida
                };
                await _perfilEventoPermissaoRepository.AddAsync(novaPermissao);
            }
        }
    }

    public async Task<Dictionary<string, bool>> ObterConfiguracaoPerfilAsync(int idEvento, int idPerfil)
    {
        var permissoesEvento = await _perfilEventoPermissaoRepository
            .GetByPerfilEventoAsync(idPerfil, idEvento);

        var resultado = new Dictionary<string, bool>();

        foreach (var permissaoEvento in permissoesEvento)
        {
            var permissao = await _permissaoRepository.GetByIdAsync(permissaoEvento.IdPermissao);
            if (permissao != null)
            {
                resultado[permissao.Chave] = permissaoEvento.Concedida;
            }
        }

        return resultado;
    }

    public async Task RemoverConfiguracaoPerfilAsync(int idEvento, int idPerfil)
    {
        var permissoes = await _perfilEventoPermissaoRepository
            .GetByPerfilEventoAsync(idPerfil, idEvento);

        foreach (var permissao in permissoes)
        {
            await _perfilEventoPermissaoRepository.RemoveAsync(permissao);
        }
    }
}
