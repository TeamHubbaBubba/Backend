using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService, UserManager<UserEntity> userManager) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private readonly UserManager<UserEntity> _userManager = userManager;
    
    [HttpGet("current-user")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SessionModel>>> GetCurrentUserBookingsAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); //Using ClaimTypes to get current user - choose one and use for all methos.
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        var result = await _bookingService.GetBookedSessionsByUserIdAsync(userId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { message = result.ResultMessage });

        return Ok(result);
    }

    [HttpPost("sessions/{sessionId}/bookings")]
    [Authorize]
    public async Task<IActionResult> CreateBooking(string sessionId)
    {
        var user = await _userManager.GetUserAsync(User); //Using user manager to get current user - choose one and use for all methos.
        if (user == null)
            return Unauthorized();

        var createdBooking = await _bookingService.CreateBookingAsync(sessionId, user.Id);

        return createdBooking.Success
            ? Ok(createdBooking.Success)
            : StatusCode(createdBooking.StatusCode, createdBooking.ResultMessage);
    }
}