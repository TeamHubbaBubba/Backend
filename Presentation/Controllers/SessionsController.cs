using Business.Interfaces;
using Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    private readonly ISessionService _sessionService = sessionService;

    //GET
    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSessionById(string sessionId)
    {
        var result = await _sessionService.GetSessionByIdAsync(sessionId);

        if (!result.Success)
        {
            switch(result.StatusCode)
            {
                case 400:
                    return BadRequest(new { message = result.ResultMessage });
                case 404:
                    return NotFound(new { message = result.ResultMessage });
                default:
                    return StatusCode(500, new { message = result.ResultMessage });
            }

        }
        //This part cast the ResponseResult to ResponseResult<SessionModel> to access the Data property
        //Then we return the data(model) not the whole ResponseResult
        if (result.Success)
            return Ok(result);

        //Safeproof - should never happen that we get a true result.Success but not a ResponseResult<T>
        return StatusCode(500, new { message = "An unexpected error occurred." });
    }
}
