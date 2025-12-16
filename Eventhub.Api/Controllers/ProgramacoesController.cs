using Eventhub.Api.Models;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProgramacoesController : BaseController
{
    private readonly IProgramacaoEventoService _programacaoEventoService;
    private readonly IProgramacaoEventoRepository _programacaoEventoRepository;

    public ProgramacoesController(IProgramacaoEventoService programacaoEventoService, IProgramacaoEventoRepository programacaoEventoRepository)
    {
        _programacaoEventoService = programacaoEventoService;
        _programacaoEventoRepository = programacaoEventoRepository;
    }

    /// <summary>
    /// Obtém todas as programações
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ProgramacaoEvento>>), 200)]
    public async Task<IActionResult> ObterTodas()
    {
        try
        {
            var programacoes = await _programacaoEventoRepository.GetAllAsync();
            return CustomResponse(programacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém uma programação por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEvento>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var programacao = await _programacaoEventoRepository.GetByIdAsync(id);
            if (programacao == null)
                return CustomResponse<object>(404, "Programação não encontrada.");

            return CustomResponse(programacao);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém programações de um evento
    /// </summary>
    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ProgramacaoEvento>>), 200)]
    public async Task<IActionResult> ObterPorEvento(int idEvento)
    {
        try
        {
            var programacoes = await _programacaoEventoService.ObterPorEventoAsync(idEvento);
            return CustomResponse(programacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cria uma nova programação
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEvento>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] ProgramacaoEvento programacao)
    {
        try
        {
            var programacaoCreated = await _programacaoEventoService.AdicionarAsync(programacao);
            return CustomResponse(programacaoCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Atualiza uma programação existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEvento>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProgramacaoEvento programacao)
    {
        try
        {
            if (id != programacao.Id)
                return CustomResponse<object>(400, "ID da programação não corresponde.");

            var programacaoAtualizada = await _programacaoEventoService.AtualizarAsync(programacao);
            return CustomResponse(programacaoAtualizada);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove uma programação
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _programacaoEventoService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
