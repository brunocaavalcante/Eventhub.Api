using Eventhub.Domain.Exceptions;

namespace Eventhub.Api.Models;

public class CustomResponse<T>
{
    public int StatusHttp { get; set; }
    public T? Data { get; set; }
    public bool ExecutouComSucesso { get; set; }
    public List<ExceptionValidationDto> Erros { get; set; }

    public CustomResponse()
    {
        Erros = new List<ExceptionValidationDto>();
    }

    public CustomResponse(T data, int statusHttp = 200)
    {
        Data = data;
        StatusHttp = statusHttp;
        ExecutouComSucesso = true;
        Erros = new List<ExceptionValidationDto>();
    }

    public CustomResponse(int statusHttp, params string[] erros)
    {
        StatusHttp = statusHttp;
        ExecutouComSucesso = false;
        Erros = erros.Select(e => ExceptionValidationDto.FromMessage(e, true)).ToList();
    }

    public CustomResponse(int statusHttp, ExceptionValidation ex)
    {
        StatusHttp = statusHttp;
        ExecutouComSucesso = false;
        Erros = new List<ExceptionValidationDto> { ExceptionValidationDto.FromException(ex) };
    }
}
