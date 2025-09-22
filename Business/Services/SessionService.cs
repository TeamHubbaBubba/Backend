using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository, IResponseResult responseResult, ISessionFactory sessionFactory) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly IResponseResult _responseResult = responseResult;
    private readonly ISessionFactory _sessionFactory = sessionFactory;

    public async Task<IResponseResult> GetSessionByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
        {
            return _responseResult.BadRequest("Id not provided by request");
        }
        try 
        {
            var result = await _sessionRepository.GetAsync(x => x.Id == id);
            if (result == null) 
            {
                return _responseResult.NotFound("We couldn't find a session with that id in the database");
            }
            var model = _sessionFactory.ToModel(result);
            return ResponseResult<SessionModel>.Ok(model);
        }
        catch (Exception ex) 
        {
            return _responseResult.BadRequest(ex.Message);
        }
    }
}