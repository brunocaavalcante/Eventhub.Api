using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContribuicaoPresenteController : BaseController
{
    private readonly IContribuicaoPresenteService _contribuicaoPresenteService;
    private readonly IUnitOfWork _unitOfWork;

    public ContribuicaoPresenteController(IContribuicaoPresenteService contribuicaoPresenteService, IUnitOfWork unitOfWork)
    {
        _contribuicaoPresenteService = contribuicaoPresenteService;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<ContribuicaoPresenteDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> CriarContribuicao([FromBody] CreateContribuicaoPresenteDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var contribuicao = await _contribuicaoPresenteService.CriarAsync(dto);
            await _unitOfWork.CommitTransactionAsync();
            return CustomResponse(contribuicao, 201);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cancela uma contribuição de presente
    /// </summary>
    [HttpPost("cancelar")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Cancelar([FromBody] CancelarContribuicaoPresenteDto dto)
    {
        try
        {
            await _contribuicaoPresenteService.CancelarAsync(dto);
            return CustomResponse<object>(200);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
