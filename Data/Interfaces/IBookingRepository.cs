using Data.Entities;

namespace Data.Interfaces;

public interface IBookingRepository : IBaseRepository<BookingEntity>
{
    Task<IEnumerable<SessionEntity>> GetBookingsByUserIdAsync(Guid id);
}