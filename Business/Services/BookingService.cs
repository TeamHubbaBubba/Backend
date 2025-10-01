
using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;

namespace Business.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    
    public async Task<ResponseResult> CreateBookingAsync(string sessionId, Guid userId)
    {
        if (sessionId == null)
            return ResponseResult.BadRequest("Session Id required.");

        try
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
        catch
        {
            return ResponseResult.Error("Unexpected error. Please try again.");
        }
    }
}
