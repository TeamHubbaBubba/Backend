using Business.Dtos;
using Business.Models;

namespace Business.Interfaces;

public interface IUserService
{
    Task<ResponseResult> CreateUserAsync(UserSignUpDto form);
    Task<ResponseResult<IEnumerable<UserModel>>> GetUsersAsync();
}