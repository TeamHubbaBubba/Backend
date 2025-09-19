
namespace Business.Models;

public class  ErrorResult : ResponseResult
{
    public ErrorResult(int statusCode, string message)
    {
        Success = false;
        StatusCode = statusCode;
        ResultMessage = message;
    }
}