using Eventhub.Api.Models;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GaleriasController : BaseController
{
    private readonly IGaleriaService _galeriaService;
    private readonly IGaleriaRepository _galeriaRepository;

    public GaleriasController(IGaleriaService galeriaService, IGaleriaRepository galeriaRepository)
    {
        _galeriaService = galeriaService;
        _galeriaRepository = galeriaRepository;
    }

    /// <summary>
    /// Obtém todas as fotos da galeria
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Galeria>>), 200)]
    public async Task<IActionResult> ObterTodas()
    {
        try
        {
            var galerias = await _galeriaRepository.GetAllAsync();
            return CustomResponse(galerias);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém uma foto da galeria por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Galeria>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var galeria = await _galeriaRepository.GetByIdAsync(id);
            if (galeria == null)
                return CustomResponse<object>(404, "Foto da galeria não encontrada.");

            return CustomResponse(galeria);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém fotos da galeria por evento
    /// </summary>
    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Galeria>>), 200)]
    public async Task<IActionResult> ObterPorEvento(int idEvento)
    {
        try
        {
            var galerias = await _galeriaService.ObterPorEventoAsync(idEvento);
            return CustomResponse(galerias);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Adiciona uma foto à galeria
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<Galeria>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] Galeria galeria)
    {
        try
        {
            var galeriaCreated = await _galeriaService.AdicionarAsync(galeria);
            return CustomResponse(galeriaCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza uma foto da galeria
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Galeria>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Galeria galeria)
    {
        try
        {
            if (id != galeria.Id)
                return CustomResponse<object>(400, "ID da galeria não corresponde.");

            var galeriaAtualizada = await _galeriaService.AtualizarAsync(galeria);
            return CustomResponse(galeriaAtualizada);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove uma foto da galeria
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _galeriaService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
