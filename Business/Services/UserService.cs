using Business.Dtos;
using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

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

        // Check if there is any user with the same email.
        var existing = await _userManager.FindByEmailAsync(form.Email);
        if (existing != null)
            return ResponseResult.AlreadyExists("A user with this email already exists.");

        // Checks if its the first user or if any already exists. If so, make the user an admin as well.
        var isFirstUser = !await _userManager.Users.AnyAsync();

        // map to entity
        var userEntity = UserFactory.Create(form);
        userEntity.UserName = form.Email;

        var result = await _userManager.CreateAsync(userEntity, form.Password);
        if (!result.Succeeded)
            return ResponseResult.Error(string.Join("; ", result.Errors.Select(e => e.Description)));


        // Adding role as either regular user or admin.
        var roleErrors = new List<string>();

        if (isFirstUser)
        {
            var adminRoleResult = await _userManager.AddToRoleAsync(userEntity, "Admin");
            if (!adminRoleResult.Succeeded)
                roleErrors.AddRange(adminRoleResult.Errors.Select(e => e.Description));
        }
        else
        {
            var userRoleResult = await _userManager.AddToRoleAsync(userEntity, "User");
            if (!userRoleResult.Succeeded)
                roleErrors.AddRange(userRoleResult.Errors.Select(e => e.Description));
        }

        // If role assignment failed, delete the created user to maintain consistency
        if (roleErrors.Count > 0)
        {
            await _userManager.DeleteAsync(userEntity);
            return ResponseResult.Error("User was not created - failed to assign roles: " + string.Join("; ", roleErrors));
        }

        return ResponseResult.Ok();

    }


    public async Task<ResponseResult<IEnumerable<UserModel>>> GetUsersAsync()
    {
        var userEntities = await _userManager.Users.ToListAsync();

        var users = userEntities.Select(UserFactory.Create).ToList();
        return ResponseResult<IEnumerable<UserModel>>.Ok(users);

    }
}