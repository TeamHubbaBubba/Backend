
using Business.Models;
using Data.Entities;

namespace Business.Factories;
public static class SessionFactory
{
    public static SessionModel EntityToModel(SessionEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        MaxParticipants = entity.MaxParticipants,
        CurrentParticipants = entity.CurrentParticipants,
        Date = entity.Date
    };
}