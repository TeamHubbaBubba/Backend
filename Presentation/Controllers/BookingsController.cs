using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService,UserManager<UserEntity> userManager) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;
    private readonly UserManager<UserEntity> _userManager = userManager;

    [HttpPost("sessions/{sessionId}/bookings")]
    public async Task<IActionResult> CreateBooking(string sessionId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var createdBooking = await _bookingService.CreateBookingAsync(sessionId, user.Id);

        return createdBooking.Success
            ? Ok(createdBooking.Success)
            : BadRequest();
    }
}
