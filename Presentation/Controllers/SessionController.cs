using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionController(ISessionService isessionService) : ControllerBase
{
    private readonly ISessionService _sessionService = isessionService;


    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] SessionDto form)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdSession = await _sessionService.CreateSessionAsync(form);

        return createdSession.Success
            ? Ok(createdSession.Success)
            : BadRequest();
    }

}
