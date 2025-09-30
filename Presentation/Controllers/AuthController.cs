using Business.Dtos;
using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, UserManager<UserEntity> userManager) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly UserManager<UserEntity> _userManager = userManager;


    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] UserSignInDto form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(form.Email);
        if (user == null)
            return Unauthorized(new { Message = "Invalid email or password." });

        await _authService.SignIn(form);

        return Ok();
    }

    [HttpPost("signout")]
    public new async Task<IActionResult> SignOut()
    {
        await _authService.SignOut();
        return Ok();
    }


}