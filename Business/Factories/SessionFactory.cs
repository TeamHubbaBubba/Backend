
using Business.Dtos;
using Data.Entities;

namespace Business.Factories;
public class SessionFactory
{
    public static SessionEntity ToEntity(SessionDto dto)
    {

        var entity = new SessionEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            MaxParticipants = dto.MaxParticipants,
            CurrentParticipants = dto.CurrentParticipants,
            Date = dto.Date
        };

        return entity;
    }

}