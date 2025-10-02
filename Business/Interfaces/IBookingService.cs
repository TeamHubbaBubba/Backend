using Business.Models;

namespace Business.Interfaces;

public interface IBookingService
{
    Task<ResponseResult> GetBookedSessionsByUserIdAsync(string userId);
}