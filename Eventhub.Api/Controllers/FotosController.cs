using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FotosController : BaseController
{
    private readonly IFotosService _fotosService;

    public FotosController(IFotosService fotosService)
    {
        _fotosService = fotosService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(CustomResponse<FotoDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Upload([FromBody] UploadFotoDto dto)
    {
        try
        {
            var foto = await _fotosService.UploadAsync(dto);
            return CustomResponse(foto, 201);
        }
        catch (ExceptionValidation ex)
        {
            return CustomResponse<object>(400, ex.Message);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<FotoDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFotoDto dto)
    {
        try
        {
            if (id != dto.Id)
                return CustomResponse<object>(400, "Id da foto não corresponde.");
            var foto = await _fotosService.UpdateAsync(dto);
            return CustomResponse(foto);
        }
        catch (ExceptionValidation ex)
        {
            return CustomResponse<object>(400, ex.Message);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<FotoDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var foto = await _fotosService.GetByIdAsync(id);
            if (foto == null)
                return CustomResponse<object>(404, "Foto não encontrada.");
            return CustomResponse(foto);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _fotosService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
