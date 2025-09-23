using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository, ISessionFactory sessionFactory) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly ISessionFactory _sessionFactory = sessionFactory;

    public async Task<ResponseResult> GetSessionByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            return ResponseResult.BadRequest("Id not provided");

        try 
        {
            var result = await _sessionRepository.GetAsync(x => x.Id == id);

            if (result == null) 
                return ResponseResult.NotFound("We couldn't find a session with that id in the database");

            var model = _sessionFactory.EntityToModel(result);
            return ResponseResult<SessionModel>.Ok(model);
        }
        //Catches and propagates any exception that might occur in the repository layer
        catch (Exception) 
        {
            return ResponseResult.Error("An unexpected error occurred. Please try again later.");
        }
    }
}