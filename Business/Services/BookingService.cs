using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<ResponseResult> GetBookedSessionsByUserIdAsync(string userId)
    {
        var sessionEntities = await _bookingRepository.GetBookingsByUserIdAsync(userId);

        if (!sessionEntities.Any())
            return ResponseResult.NotFound("No booked sessions found");

        var bookedSessions = sessionEntities.Select(SessionFactory.EntityToModel);

        return ResponseResult<IEnumerable<SessionModel>>.Ok(bookedSessions);
    }
}
