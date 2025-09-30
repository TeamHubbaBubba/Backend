using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class AuthService(SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly SignInManager<UserEntity> _signInManager = signInManager;


    public async Task<bool> SignIn(UserSignInDto form)
    {
        var userSignIn = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
        return userSignIn.Succeeded;
    }

    public async Task SignOut()
    {
        await _signInManager.SignOutAsync();
    }
}