using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    private readonly ISessionService _sessionService = sessionService;

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var result = await _sessionService.DeleteSessionAsync(x => x.Id == id);

        if (!result.Success)
            return StatusCode(result.StatusCode, new { message = result.ResultMessage });

        return Ok();
    }
}
