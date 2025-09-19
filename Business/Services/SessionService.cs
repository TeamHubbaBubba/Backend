using Business.Interfaces;
using Data.Interfaces;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;

}