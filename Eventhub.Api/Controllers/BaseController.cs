using Eventhub.Api.Models;
using Eventhub.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Eventhub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult CustomResponse<T>(T data, int statusCode = 200)
    {
        var response = new CustomResponse<T>(data, statusCode);
        return StatusCode(statusCode, response);
    }

    protected IActionResult CustomResponse<T>(int statusCode, params string[] erros)
    {
        var response = new CustomResponse<T>(statusCode, erros);
        return StatusCode(statusCode, response);
    }

    protected IActionResult CustomResponse<T>(int statusCode, List<string> erros)
    {
        var response = new CustomResponse<T>(statusCode, erros);
        return StatusCode(statusCode, response);
    }

    protected IActionResult TratarErros(Exception ex)
    {
        if (ex is ExceptionValidation validationEx)
        {
            return CustomResponse<object>(400, validationEx.Message);
        }

        return CustomResponse<object>(500, ex.Message);
    }
}
