using Business.Models;

namespace Business.Interfaces;

public interface ISessionService
{
    Task<ResponseResult> GetAllSessionsAsync();
}