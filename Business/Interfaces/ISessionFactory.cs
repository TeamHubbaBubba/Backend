using Business.Models;
using Data.Entities;

namespace Business.Interfaces;
public interface ISessionFactory
{
    SessionModel ToModel(SessionEntity entity);
}