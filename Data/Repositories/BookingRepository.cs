using Data.Contexts;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BookingRepository(DataContext context) : BaseRepository<BookingEntity>(context), IBookingRepository
{
    public async Task<IEnumerable<SessionEntity>> GetBookingsByUserIdAsync(string id)
    {
        var bookedSessions = await _context.Bookings
            .Where(x => x.UserId.ToString() == id)
            .Select(s => s.Session)
            .OrderBy(d => d.Date)
            .ToListAsync();

        return bookedSessions;
    }
}
