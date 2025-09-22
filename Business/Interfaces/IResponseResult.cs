using Business.Models;

namespace Business.Interfaces;

public interface IResponseResult
{
    string? ResultMessage { get; }
    int StatusCode { get; }
    bool Success { get; }

    ResponseResult AlreadyExists(string message);
    ResponseResult BadRequest(string message);
    ResponseResult Error(string message);
    ResponseResult NotFound(string message);
    ResponseResult Ok();
}

public interface IResponseResult<T>
{
    T? Data { get; }

    static abstract ResponseResult<T> Ok(T? data);
}