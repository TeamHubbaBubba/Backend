using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Business.Dtos;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<ResponseResult> GetBookedSessionsByUserIdAsync(string userId)
    {
        var sessionEntities = await _bookingRepository.GetBookingsByUserIdAsync(userId);

        if (!sessionEntities.Any())
            return ResponseResult.NotFound("No booked sessions found");

        var bookedSessions = sessionEntities.Select(SessionFactory.EntityToModel);

        return ResponseResult<IEnumerable<SessionModel>>.Ok(bookedSessions);
    }
  
    public async Task<ResponseResult> CreateBookingAsync(string sessionId, Guid userId)
    {
        if (sessionId == null)
            return ResponseResult.BadRequest("Session Id required.");

        try
        {
            SessionModel updatedSession = new SessionModel();

            var sessionResult = await _sessionService.GetSessionByIdAsync(sessionId);

            if (sessionResult is ResponseResult<SessionModel> ok && ok.Success && ok.Data is { } sessionModel)
            {
                if (sessionModel.CurrentParticipants == sessionModel.MaxParticipants)
                {
                    return ResponseResult.Error("Fully Booked Session!");
                }

                sessionModel.CurrentParticipants = sessionModel.CurrentParticipants+1;

                updatedSession = sessionModel;
            }
            var updateResult = await _sessionService.UpdateSessionAsync(updatedSession);

            if (updateResult.Success)
            {
                var booking = new BookingEntity
                {
                    UserId = userId,
                    SessionId = sessionId
                };

                var result = await _bookingRepository.CreateAsync(booking);

                if (result == null)
                    return ResponseResult.Error("Unable to create booking. Please try again.");


                return ResponseResult.Ok();
            }

            return ResponseResult.Error("Unexpected error. Please try again.");

        }
        catch
        {
            return ResponseResult.Error("Unexpected error. Please try again.");
        }
    }
}