using Data.Entities;
using System.Linq.Expressions;
using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface ISessionService
{

    Task<ResponseResult> DeleteSessionAsync(Expression<Func<SessionEntity, bool>> expression);
    Task<ResponseResult> GetSessionByIdAsync(string id);
    Task<ResponseResult> GetAllSessionsAsync();
    Task<ResponseResult> CreateSessionAsync(SessionDto form);
    Task<ResponseResult> UpdateSessionAsync(SessionModel session);
}