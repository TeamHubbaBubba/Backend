using Business.Dtos;
using Data.Entities;

using Business.Models;
namespace Business.Factories;

public class UserFactory
{
    public static UserSignUpDto Create() => new();

    public static UserEntity Create(UserSignUpDto form) => new()
    {
        FirstName = form.FirstName,
        LastName = form.LastName,
        Email = form.Email,
        
    };

    public static UserModel Create(UserEntity entity) => new()
    {
        Id = entity.Id,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
    };
}
