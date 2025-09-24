using Business.Models;
using Business.Dtos;
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