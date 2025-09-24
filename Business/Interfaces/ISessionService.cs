using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface ISessionService
{
    Task<ResponseResult> GetAllSessionsAsync();
    Task<ResponseResult> CreateSessionAsync(SessionDto form);
}