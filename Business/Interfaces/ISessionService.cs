using Business.Models;
using Data.Entities;
using System.Linq.Expressions;

namespace Business.Interfaces;

public interface ISessionService
{
    Task<ResponseResult> DeleteSessionAsync(Expression<Func<SessionEntity, bool>> expression);
}