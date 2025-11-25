using Eventhub.Api.Models;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Domain.Entities;
using Eventhub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : BaseController
{
    private readonly IUsuarioService _usuarioService;
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuariosController(IUsuarioService usuarioService, IUsuarioRepository usuarioRepository)
    {
        _usuarioService = usuarioService;
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(CustomResponse<IEnumerable<Usuario>>), 200)]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return CustomResponse(usuarios);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Usuario>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
                return CustomResponse<object>(404, "Usuário não encontrado.");

            return CustomResponse(usuario);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(CustomResponse<Usuario>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> ObterPorEmail(string email)
    {
        try
        {
            var usuario = await _usuarioService.ObterPorEmailAsync(email);
            if (usuario == null)
                return CustomResponse<object>(404, "Usuário não encontrado.");

            return CustomResponse(usuario);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomResponse<CreateUsuarioDto>), 201)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    public async Task<IActionResult> Criar([FromBody] CreateUsuarioDto usuario)
    {
        try
        {
            var usuarioCreated = await _usuarioService.AdicionarAsync(usuario);
            return CustomResponse(usuarioCreated, 201);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CustomResponse<Usuario>), 200)]
    [ProducesResponseType(typeof(CustomResponse<object>), 400)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Usuario usuario)
    {
        try
        {
            if (id != usuario.Id)
                return CustomResponse<object>(400, "ID do usuário não corresponde.");

            var usuarioAtualizado = await _usuarioService.AtualizarAsync(usuario);
            return CustomResponse(usuarioAtualizado);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }

    /// <summary>
    /// Remove um usuário
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(CustomResponse<object>), 204)]
    [ProducesResponseType(typeof(CustomResponse<object>), 404)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _usuarioService.RemoverAsync(id);
            return CustomResponse<object>(new { }, 204);
        }
        catch (Exception ex)
        {
            return TratarErros(ex);
        }
    }
}
