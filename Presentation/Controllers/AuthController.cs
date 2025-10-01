using Business.Interfaces;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [Route("signout")]
        [HttpPost]
        public async Task<IActionResult> SignOutAsync()
        {
            var result = await _authService.SignOutAsync();
            if (!result.Success)
                return StatusCode(500, result);
                
            return Ok(result);
        }
    }
}
