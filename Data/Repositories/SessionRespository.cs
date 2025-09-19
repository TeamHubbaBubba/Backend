using Data.Contexts;
using Data.Entities;
using Data.Interfaces;

namespace Data.Repositories;

public class SessionRespository (DataContext context) : BaseRepository<SessionEntity>(context), ISessionRepository
{

}
