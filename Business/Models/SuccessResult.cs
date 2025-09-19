
namespace Business.Models;

public class SuccessResult : ResponseResult
{
    public SuccessResult (int statusCode)
    {
        Success = true;
        StatusCode = statusCode;
    }
}
