using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using System.Diagnostics;

namespace Business.Services;

public class SessionService(ISessionRepository sessionRepository) : ISessionService
{
    private readonly ISessionRepository _sessionRepository = sessionRepository;

    public async Task<ResponseResult> GetAllSessionsAsync()
    {
        var sessionEntities = await _sessionRepository.GetAllAsync();

        if (sessionEntities == null || sessionEntities.Count() <= 0)
            return ResponseResult.NotFound("No Sessions found");

        var sessions = sessionEntities.Select(SessionFactory.EntityToModel);

        return ResponseResult<IEnumerable<SessionModel>>.Ok(sessions);
    }

    //Create session method

    public async Task<ResponseResult> CreateSessionAsync(SessionDto form)
    {
        if (form == null)
            return ResponseResult.BadRequest("Invalid form");

        try
        {
            var sessionEntity = SessionFactory.ToEntity(form);
            var result = await _sessionRepository.CreateAsync(sessionEntity);

            if (result == null)
                return ResponseResult.BadRequest("Enter all required fields.");

            return ResponseResult<SessionEntity>.Ok(result);
        }
        catch
        {
            return ResponseResult.Error("Failed to create session");
        }
    }
}