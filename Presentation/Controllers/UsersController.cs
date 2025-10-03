using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Business.Interfaces;
using Business.Dtos;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{

    private readonly IUserService _userService = userService;


    [HttpPost]
    public IActionResult CreateUser(UserSignUpDto form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdUser = _userService.CreateUserAsync(form);
        return createdUser.Result.Success
            ? Ok(createdUser.Result)
            : createdUser.Result.StatusCode == 409
                ? Conflict(createdUser.Result)
                : BadRequest(createdUser.Result);

    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _userService.GetUsersAsync();
        return users.Result.Success
            ? Ok(users.Result)
            : BadRequest(users.Result);
    }

}
