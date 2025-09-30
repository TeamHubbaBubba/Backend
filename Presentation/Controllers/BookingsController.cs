using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpPost("sessions/{sessionId}/bookings")]
    public async Task<IActionResult> CreateBooking(string sessionId)
    {
        var createdBooking = await _bookingService.CreateBookingAsync(sessionId);

        return createdBooking.Success
            ? Ok(createdBooking.Success)
            : BadRequest();
    }
}
