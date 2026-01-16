using System.Net;

namespace Application.Shared;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public T Data { get; set; }


    public BaseResponse(string Message, HttpStatusCode StatusCode)
    {
        this.Message = Message;
        Success = false;
        this.StatusCode = StatusCode;
    }
    public BaseResponse(string Message, bool isSuccess, HttpStatusCode StatusCode)
    {
        this.Message=Message;
        Success = isSuccess;
        this.StatusCode = StatusCode;
    }
    public BaseResponse(string Message, T? Data,  HttpStatusCode StatusCode)
    {
        this.Message = Message;
        this.Data = Data;
        this.StatusCode = StatusCode;
        this.StatusCode = StatusCode;
    }


}

