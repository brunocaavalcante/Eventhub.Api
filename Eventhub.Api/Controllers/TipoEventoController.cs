using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TipoEventoController : BaseController
{
    private readonly ITipoEventoService _tipoEventoService;

    public TipoEventoController(ITipoEventoService tipoEventoService)
    {
        _tipoEventoService = tipoEventoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<TipoEventoDto>), 200)]
    public async Task<IActionResult> ListarTodos()
    {
        try
        {
            var tipos = await _tipoEventoService.ListarTodosAsync();
            return CustomResponse(tipos);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
