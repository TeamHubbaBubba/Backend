using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController(ISessionService sessionService) : ControllerBase
    {
        private readonly ISessionService _sessionService = sessionService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();

            return sessions.Success ? Ok(sessions) : NotFound("Inga träningspass tillgängliga");
        }


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
}
