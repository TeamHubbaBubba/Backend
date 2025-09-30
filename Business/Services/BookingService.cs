
using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;

namespace Business.Services;

public class BookingService(IBookingRepository bookingRepository) : IBookingService
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<ResponseResult> CreateBookingAsync(string sessionId)
    {
        if (sessionId == null)
            return ResponseResult.BadRequest("Session Id required.");

        try
        {
            return ResponseResult.Ok();
        }
        catch
        {
            return ResponseResult.Error("Wuut??");
        }
    }
}
