using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Linq.Expressions;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public async Task<ResponseResult> DeleteSessionAsync(Expression<Func<SessionEntity, bool>> expression)
    {
        if (expression == null)
            return ResponseResult.BadRequest("Invalid request");

        try
        {
            var result = await _sessionRepository.DeleteAsync(expression);

            if (!result)
                return ResponseResult.NotFound("Session was not found");

            return ResponseResult.Ok();
        }
        catch 
        {
            return ResponseResult.Error("Error deleting session");
        }

    }
}