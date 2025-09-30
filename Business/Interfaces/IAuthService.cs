using Business.Dtos;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<bool> SignIn(UserSignInDto form);
        Task SignOut();
    }
}