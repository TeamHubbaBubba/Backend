using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingsService) : ControllerBase
{
    private readonly IBookingService _bookingsService = bookingsService;

    [HttpGet("current-user")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SessionModel>>> GetCurrentUserBookingsAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        var result = await _bookingsService.GetBookedSessionsByUserIdAsync(userId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { message = result.ResultMessage });

        return Ok(result);
    }
}
