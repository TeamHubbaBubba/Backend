using System.Linq.Expressions;
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

    public async Task<ResponseResult> GetSessionByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            return ResponseResult.BadRequest("Id not provided");

        try 
        {
            var result = await _sessionRepository.GetAsync(x => x.Id == id);

            if (result == null) 
                return ResponseResult.NotFound("We couldn't find a session with that id in the database");

            var model = SessionFactory.EntityToModel(result);
            return ResponseResult<SessionModel>.Ok(model);
        }
        //Catches and propagates any exception that might occur in the repository layer
        catch (Exception) 
        {
            return ResponseResult.Error("An unexpected error occurred. Please try again later.");
        }
    }
    
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
            sessionEntity.CurrentParticipants = form.MaxParticipants;
            var result = await _sessionRepository.CreateAsync(sessionEntity);

            if (result == null)
                return ResponseResult.BadRequest("Enter all required fields.");

            return ResponseResult.Ok(); //Ändrade så vi bara skickar ok utan entitet
        }
        catch
        {
            return ResponseResult.Error("Failed to create session");
        }
    }

    public async Task<ResponseResult> UpdateSessionAsync(SessionModel session)
    {
        if (session is null || string.IsNullOrWhiteSpace(session.Id))
            return ResponseResult.BadRequest("Invalid session payload.");

        var existingSession = await _sessionRepository.GetAsync(s => s.Id == session.Id);
        if (existingSession == null)
            return ResponseResult.NotFound("Session not found.");

        if (session.MaxParticipants < existingSession.CurrentParticipants)
            return ResponseResult.BadRequest("MaxParticipants cannot be less than CurrentParticipants.");


        var targetDate = session.Date != default ? session.Date : existingSession.Date;

        var updateSucceeded = await _sessionRepository.UpdateAsync(
            s => s.Id == session.Id,
            new SessionEntity
            {
                Id = session.Id,
                Title = session.Title,
                Description = session.Description,
                MaxParticipants = session.MaxParticipants,
                CurrentParticipants = session.CurrentParticipants,
                Date = targetDate,
                Intensity = session.Intensity
            });

        return updateSucceeded ? ResponseResult.Ok() : ResponseResult.Error("Failed to update session.");
    }



}