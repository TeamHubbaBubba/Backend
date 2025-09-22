using Business.Interfaces;
using Business.Models;
using Data.Entities;

namespace Business.Factories;
public class SessionFactory : ISessionFactory
{
    //Cast Entity to Model
    public SessionModel ToModel(SessionEntity entity)
    {
        var model = new SessionModel
        {
            Title = entity.Title,
            Description = entity.Description,
            MaxParticipants = entity.MaxParticipants,
            CurrentParticipants = entity.CurrentParticipants,
            Date = entity.Date
        };

        return model;
    }
}