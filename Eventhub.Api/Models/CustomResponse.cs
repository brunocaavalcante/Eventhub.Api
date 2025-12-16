namespace Eventhub.Api.Models;

public class CustomResponse<T>
{
    public int StatusHttp { get; set; }
    public T? Data { get; set; }
    public bool ExecutouComSucesso { get; set; }
    public List<string> Erros { get; set; }

    public CustomResponse()
    {
        Erros = new List<string>();
    }

    public CustomResponse(T data, int statusHttp = 200)
    {
        Data = data;
        StatusHttp = statusHttp;
        ExecutouComSucesso = true;
        Erros = new List<string>();
    }

    public CustomResponse(int statusHttp, params string[] erros)
    {
        StatusHttp = statusHttp;
        ExecutouComSucesso = false;
        Erros = erros.ToList();
    }

    public CustomResponse(int statusHttp, List<string> erros)
    {
        StatusHttp = statusHttp;
        ExecutouComSucesso = false;
        Erros = erros;
    }
}
