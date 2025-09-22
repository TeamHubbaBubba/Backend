using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Diagnostics;
namespace Business.Services;
public class SessionService(ISessionRepository sessionRepository, IResponseResult responseResult) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;
    private readonly IResponseResult _responseResult = responseResult;


    //Create session method

    public async Task<IResponseResult> CreateSessionAsync(SessionDto form)
    {
        if (form == null)
        {
            return _responseResult.BadRequest("Invalid form");
        }

        try
        {
            var sessionEntity = SessionFactory.ToEntity(form);
            var result = await _sessionRepository.CreateAsync(sessionEntity);

            if (result == null)
                return _responseResult.BadRequest("Enter all required fields.");

            return ResponseResult<SessionEntity>.Ok(result);
        }
        catch
        {
            return _responseResult.Error("Failed to create session");
        }
    }
}