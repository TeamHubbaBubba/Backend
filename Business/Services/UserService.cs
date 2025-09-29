using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public class UserService(UserManager<UserEntity> userManager, RoleManager<IdentityRole<Guid>> roleManager) : IUserService
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;



    public async Task<ResponseResult> CreateUserAsync(UserSignUpDto form)
    {
        if (form == null)
            return ResponseResult.BadRequest("Invalid user data.");
        if (string.IsNullOrWhiteSpace(form.Email) || string.IsNullOrWhiteSpace(form.Password))
            return ResponseResult.BadRequest("Email and password are required.");


        var existing = await _userManager.FindByEmailAsync(form.Email);
        if (existing != null)
            return ResponseResult.AlreadyExists("A user with this email already exists.");

        var userEntity = UserFactory.Create(form);
        userEntity.UserName = form.Email;

        var result = await _userManager.CreateAsync(userEntity, form.Password);
        if (result.Succeeded)
            return ResponseResult.Ok();

        return ResponseResult.Error("Failed to create user: " + string.Join("; ", result.Errors.Select(e => e.Description)));

    }









}
