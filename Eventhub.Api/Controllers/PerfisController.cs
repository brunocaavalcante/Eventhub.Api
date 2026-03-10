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

    [HttpGet("{idEvento}/{idPerfil}/modulos")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ModuloDto>>), 200)]
    public async Task<IActionResult> ObterModulosPorPerfil(int idPerfil, int idEvento)
    {
        try
        {
            var modulos = await _perfilService.ObterModulosPerfilAsync(idPerfil, idEvento);
            return CustomResponse(modulos.OrderBy(m => m.Ordem));
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}