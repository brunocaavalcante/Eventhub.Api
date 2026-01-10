using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Enums;
using Eventhub.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PixEventoController : BaseController
{
    private readonly IPixEventoService _pixEventoService;

    public PixEventoController(IPixEventoService pixEventoService)
    {
        _pixEventoService = pixEventoService;
    }

    /// <summary>
    /// Criar novo PIX para evento (substitui PIX existente se houver para mesma finalidade)
    /// </summary>
    /// <param name="dto">Dados do PIX</param>
    /// <returns>PIX criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<PixEventoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePixEventoDto dto)
    {
        try
        {
            var result = await _pixEventoService.CreateAsync(dto);
            return CustomResponse(result, StatusCodes.Status201Created);
        }
        catch (ExceptionValidation ex)
        {
            return CustomResponse<object>(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    /// <summary>
    /// Atualizar PIX existente (apenas NomeBeneficiario e QRCodePix)
    /// </summary>
    /// <param name="id">ID do PIX</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>PIX atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PixEventoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePixEventoDto dto)
    {
        try
        {
            if (id != dto.Id)
            {
                return CustomResponse<object>(StatusCodes.Status400BadRequest, "ID do PIX não corresponde ao informado na requisição.");
            }

            var result = await _pixEventoService.UpdateAsync(dto);
            return CustomResponse(result);
        }
        catch (ExceptionValidation ex)
        {
            return CustomResponse<object>(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    /// <summary>
    /// Excluir PIX
    /// </summary>
    /// <param name="id">ID do PIX</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _pixEventoService.DeleteAsync(id);
            return CustomResponse("PIX excluído com sucesso.");
        }
        catch (ExceptionValidation ex)
        {
            return CustomResponse<object>(StatusCodes.Status400BadRequest, ex.Message);
        }
    }

    /// <summary>
    /// Buscar PIX por ID
    /// </summary>
    /// <param name="id">ID do PIX</param>
    /// <returns>Dados do PIX</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<PixEventoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _pixEventoService.GetByIdAsync(id);
        if (result == null)
        {
            return CustomResponse<object>(StatusCodes.Status404NotFound, "PIX não encontrado.");
        }

        return CustomResponse(result);
    }

    /// <summary>
    /// Listar todos os PIX de um evento
    /// </summary>
    /// <param name="idEvento">ID do evento</param>
    /// <returns>Lista de PIX do evento</returns>
    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<PixEventoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEventoId(int idEvento)
    {
        var result = await _pixEventoService.GetByEventoIdAsync(idEvento);
        return CustomResponse(result);
    }

    /// <summary>
    /// Buscar PIX de um evento por finalidade (retorna único PIX)
    /// </summary>
    /// <param name="idEvento">ID do evento</param>
    /// <param name="finalidade">Finalidade do PIX</param>
    /// <returns>PIX do evento para a finalidade</returns>
    [HttpGet("evento/{idEvento}/finalidade/{finalidade}")]
    [ProducesResponseType(typeof(CustomResponse<PixEventoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEventoAndFinalidade(int idEvento, FinalidadePix finalidade)
    {
        var result = await _pixEventoService.GetByEventoAndFinalidadeAsync(idEvento, finalidade);
        
        if (result == null)
        {
            return CustomResponse<object>(StatusCodes.Status404NotFound, "PIX não encontrado para esta finalidade.");
        }

        return CustomResponse(result);
    }
}