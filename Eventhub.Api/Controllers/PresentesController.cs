using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PresentesController : BaseController
{
    private readonly IPresenteService _presenteService;
    private readonly IUnitOfWork _unitOfWork;

    public PresentesController(IPresenteService presenteService, IUnitOfWork unitOfWork)
    {
        _presenteService = presenteService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PresenteDto>>), 200)]
    public async Task<IActionResult> ListarTodos(int idEvento)
    {
        try
        {
            var presentes = await _presenteService.ListarTodosAsync(idEvento);
            return CustomResponse(presentes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("categorias")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<CategoriaPresenteDto>>), 200)]
    public async Task<IActionResult> ListarCategoriaPresentes()
    {
        try
        {
            var categorias = await _presenteService.ListarCategoriaPresentesAsync();
            return CustomResponse(categorias);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var presente = await _presenteService.ObterPorIdAsync(id);

            if (presente == null)
                return CustomResponse<object>(404, "Presente não encontrado.");

            return CustomResponse(presente);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CreatePresenteDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var presente = await _presenteService.AdicionarAsync(dto);
            await _unitOfWork.CommitTransactionAsync();
            return CustomResponse(presente, 201);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PresenteDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdatePresenteDto dto)
    {
        try
        {
            if (id != dto.Id)
                return CustomResponse<object>(400, "ID do presente não corresponde.");

            await _unitOfWork.BeginTransactionAsync();
            var presente = await _presenteService.AtualizarAsync(dto);
            await _unitOfWork.CommitTransactionAsync();

            return CustomResponse(presente);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _presenteService.RemoverAsync(id);
            await _unitOfWork.CommitTransactionAsync();
            return CustomResponse<object>(null, 200);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TratarErros(ex);
        }
    }
}