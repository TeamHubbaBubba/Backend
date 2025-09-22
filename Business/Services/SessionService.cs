using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public async Task<ResponseResult> GetAllSessionsAsync()
    {
        var sessionEntities = await _sessionRepository.GetAllAsync();

        if (sessionEntities == null)
            return ResponseResult.NotFound("No Sessions found");

        var sessions = sessionEntities.Select(SessionFactory.EntityToModel);

        return ResponseResult<IEnumerable<SessionModel>>.Ok(sessions);
    }
}