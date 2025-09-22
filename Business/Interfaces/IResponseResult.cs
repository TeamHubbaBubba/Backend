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
//I don't think we need a IResponseResult<T> since ResponseResult<T> inherits from ResponseResult,and it implements IResponseResult
//Not 100% sure tho - @Maria Spinola