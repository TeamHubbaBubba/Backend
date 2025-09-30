using Business.Dtos;
using Business.Models;

namespace Business.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseResult> CreateBookingAsync(string sessionId);
    }
}