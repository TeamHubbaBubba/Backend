
using Business.Interfaces;

namespace Business.Models;

public abstract class ResponseResult
{
    public bool Success { get; protected set; }
    public int StatusCode { get; protected set; }
    public string? ResultMessage { get; protected set; }

    public static ResponseResult Ok()
    {
        return new SuccessResult(200);
    }

    public static ResponseResult BadRequest(string message)
    {
        return new ErrorResult(400, message);
    }

    public static ResponseResult NotFound(string message)
    {
        return new ErrorResult(404, message);
    }

    public static ResponseResult AlreadyExists(string message)
    {
        return new ErrorResult(409, message);
    }

    public static ResponseResult Error(string message)
    {
        return new ErrorResult(500, message);
    }
}

public class ResponseResult<T> : ResponseResult where T : class
{
    public T? Data { get; private set; }

    public static ResponseResult<T> Ok(T? data)
    {
        return new ResponseResult<T>
        {
            Success = true,
            StatusCode = 200,
            Data = data
        };
    }
}