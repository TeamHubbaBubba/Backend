using Business.Dtos;
using Business.Interfaces;
using Business.Services;
using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
                        return BadRequest(result);
                    case 404:
                        return NotFound(result);
                    default:
                        return StatusCode(result);
                }

            }
            //This part cast the ResponseResult to ResponseResult<SessionModel> to access the Data property
            //Then we return the data(model) not the whole ResponseResult
            if (result.Success)
                return Ok(result);
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
