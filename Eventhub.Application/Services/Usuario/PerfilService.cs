using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Interfaces;

namespace Eventhub.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly IPerfilRepository _perfilRepository;
    private readonly IModuloRepository _moduloRepository;
    private readonly IPerfilEventoPermissaoRepository _perfilEventoPermissaoRepository;
    private readonly IMapper _mapper;

    public PerfilService(IPerfilRepository perfilRepository, IModuloRepository moduloRepository,
    IPerfilEventoPermissaoRepository perfilEventoPermissaoRepository, IMapper mapper)
    {
        _perfilRepository = perfilRepository;
        _moduloRepository = moduloRepository;
        _perfilEventoPermissaoRepository = perfilEventoPermissaoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PerfilDto>> ObterPerfisAtivosAsync()
    {
        var perfis = await _perfilRepository.GetPerfisAtivosAsync();
        return _mapper.Map<IEnumerable<PerfilDto>>(perfis);
    }

    public async Task<IEnumerable<ModuloDto>> ObterModulosPerfilAsync(int idPerfil, int idEvento)
    {
        if (idEvento == 0) throw new ValidationException("O id do evento deve ser informado para obter as permissões do perfil.");

        if (idPerfil == 0) idPerfil = (int)EnumPerfil.Convidado;

        var modulos = await _moduloRepository.FindAsync(m => m.ShowInMenu);

        if (idPerfil == (int)EnumPerfil.Administrador)
            return _mapper.Map<IEnumerable<ModuloDto>>(modulos.OrderBy(m => m.Ordem));

        var permissoesPerfil = await _perfilEventoPermissaoRepository.GetByPerfilEventoAsync(idPerfil, idEvento);

        if (permissoesPerfil == null || permissoesPerfil.Count() == 0)
        {
            permissoesPerfil = await _perfilEventoPermissaoRepository.GetByPerfilEventoAsync((int)EnumPerfil.Convidado, idEvento);
        }

        var permissoesConcedidas = permissoesPerfil.Where(p => modulos.Any(m => m.Id == p.Permissao.IdModulo) &&
                                                             p.Concedida && p.Perfil.Status == 'A').ToList();

        modulos = modulos.Where(m => permissoesConcedidas.Any(p => p.Permissao.IdModulo == m.Id)).ToList();

        return _mapper.Map<IEnumerable<ModuloDto>>(modulos);
    }
}