using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Exceptions;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

/// <summary>
/// Controller para gerenciamento de programação de eventos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgramacaoEventoController : BaseController
{
    private readonly IProgramacaoEventoService _programacaoEventoService;
    private readonly IEventoRepository _eventoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ProgramacaoEventoController(
        IProgramacaoEventoService programacaoEventoService,
        IEventoRepository eventoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _programacaoEventoService = programacaoEventoService;
        _eventoRepository = eventoRepository;
        _usuarioRepository = usuarioRepository;
    }

    private async Task<int> ObterIdUsuarioLogadoAsync()
    {
        var auth0Id = User.FindFirst("sub")?.Value
                   ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(auth0Id))
            throw new ExceptionValidation("Claim de identificação não encontrado no token.", true);

        var usuario = await _usuarioRepository.GetByKeycloakIdAsync(auth0Id);

        if (usuario == null)
            throw new ExceptionValidation("Usuário não cadastrado no sistema.", true);

        return usuario.Id;
    }

    /// <summary>
    /// Obtém todas as programações de um evento
    /// </summary>
    [HttpGet("evento/{idEvento}")]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<ProgramacaoEventoResponseDto>>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 403)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorEvento(int idEvento)
    {
        try
        {
            var idUsuarioLogado = await ObterIdUsuarioLogadoAsync();

            var evento = await _eventoRepository.GetByIdAsync(idEvento);
            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado.");

            // Verificar se o usuário tem acesso ao evento (criador ou participante)
            var participante = evento.Participantes?.FirstOrDefault(p => p.IdUsuario == idUsuarioLogado);
            if (evento.IdUsuarioCriador != idUsuarioLogado && participante == null)
                return CustomResponse<object>(403, "Você não tem permissão para visualizar a programação deste evento.");

            var programacoes = await _programacaoEventoService.ObterPorEventoAsync(idEvento);
            return CustomResponse(programacoes);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Obtém uma programação específica por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEventoResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 403)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var idUsuarioLogado = await ObterIdUsuarioLogadoAsync();
            var programacao = await _programacaoEventoService.ObterPorIdAsync(id);

            var evento = await _eventoRepository.GetByIdAsync(programacao.IdEvento);
            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado.");

            // Verificar se o usuário tem acesso ao evento
            var participante = evento.Participantes?.FirstOrDefault(p => p.IdUsuario == idUsuarioLogado);
            if (evento.IdUsuarioCriador != idUsuarioLogado && participante == null)
                return CustomResponse<object>(403, "Você não tem permissão para visualizar esta programação.");

            return CustomResponse(programacao);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Cria uma nova programação para o evento
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEventoResponseDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 403)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Criar([FromBody] ProgramacaoEventoCreateDto dto)
    {
        try
        {
            var idUsuarioLogado = await ObterIdUsuarioLogadoAsync();

            var evento = await _eventoRepository.GetByIdAsync(dto.IdEvento);
            if (evento == null) throw new ExceptionValidation("Evento não encontrado.");

            // Apenas o criador do evento pode adicionar programações
            if (evento.IdUsuarioCriador != idUsuarioLogado)
                throw new ExceptionValidation("Apenas o criador do evento pode adicionar programações.", true);

            var programacao = await _programacaoEventoService.AdicionarAsync(dto);
            return CustomResponse(programacao, 201);
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
    [ProducesResponseType(typeof(CustomResponse<ProgramacaoEventoResponseDto>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 403)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProgramacaoEventoUpdateDto dto)
    {
        try
        {
            if (id != dto.Id)
                return CustomResponse<object>(400, "O ID da URL não corresponde ao ID do corpo da requisição.");

            var idUsuarioLogado = await ObterIdUsuarioLogadoAsync();

            var programacaoExistente = await _programacaoEventoService.ObterPorIdAsync(id);
            var evento = await _eventoRepository.GetByIdAsync(programacaoExistente.IdEvento);

            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado.");

            // Apenas o criador do evento pode atualizar programações
            if (evento.IdUsuarioCriador != idUsuarioLogado)
                return CustomResponse<object>(403, "Apenas o criador do evento pode atualizar programações.");

            var programacao = await _programacaoEventoService.AtualizarAsync(dto);
            return CustomResponse(programacao);
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
    [ProducesResponseType(typeof(CustomResponse<object>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 403)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            var idUsuarioLogado = await ObterIdUsuarioLogadoAsync();

            var programacaoExistente = await _programacaoEventoService.ObterPorIdAsync(id);
            var evento = await _eventoRepository.GetByIdAsync(programacaoExistente.IdEvento);

            if (evento == null)
                return CustomResponse<object>(404, "Evento não encontrado.");

            // Apenas o criador do evento pode remover programações
            if (evento.IdUsuarioCriador != idUsuarioLogado)
                return CustomResponse<object>(403, "Apenas o criador do evento pode remover programações.");

            await _programacaoEventoService.RemoverAsync(id);
            return CustomResponse<object>(true, 200);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
