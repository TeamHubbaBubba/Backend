using Business.Models;

namespace Business.Interfaces;

public interface IBookingService
{
    Task<ResponseResult> GetBookedSessionsByUserIdAsync(string userId);
    Task<ResponseResult> CreateBookingAsync(string sessionId, Guid userId);

}