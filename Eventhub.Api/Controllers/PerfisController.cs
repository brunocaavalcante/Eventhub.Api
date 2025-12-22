using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerfisController : BaseController
{
    private readonly IPerfilService _perfilService;

    public PerfisController(IPerfilService perfilService)
    {
        _perfilService = perfilService;
    }

    [HttpGet("ativos")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PerfilDto>>), 200)]
    public async Task<IActionResult> ObterPerfisAtivos()
    {
        try
        {
            var perfisDto = await _perfilService.ObterPerfisAtivosAsync();
            return CustomResponse(perfisDto);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{idPerfil}/permissoes")]
    [ProducesResponseType(typeof(CustomResponse<PermissoesPerfilDto>), 200)]
    public async Task<IActionResult> ObterPermissoesPerfil(int idPerfil)
    {
        try
        {
            var permissoesDto = await _perfilService.ObterPermissoesPerfilAsync(idPerfil);
            return CustomResponse(permissoesDto);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{idPerfil}/modulos")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ModuloDto>>), 200)]
    public async Task<IActionResult> ObterModulosPorPerfil(int idPerfil)
    {
        try
        {
            var modulos = new List<ModuloDto>();
            var permissoesDto = await _perfilService.ObterPermissoesPerfilAsync(idPerfil);

            foreach (var permissao in permissoesDto.Permissoes)
            {
                if (!modulos.Any(m => m.Id == permissao.Modulo.Id))
                    modulos.Add(permissao.Modulo);
            }

            return CustomResponse(modulos.OrderBy(m => m.Ordem));
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}