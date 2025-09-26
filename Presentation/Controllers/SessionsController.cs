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
                        return StatusCode(500 ,result);
                }

            }
            
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

        [HttpPost]
        public async Task<IActionResult> UpdateAsync([FromBody] string id, SessionDto form) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var sessionToUpdate = await _sessionService.GetSessionByIdAsync(id);
            if (!sessionToUpdate.Success)
                return NotFound(new { message = "Session not found" });
            var sessionToUpdateData = sessionToUpdate as ResponseResult<SessionModel>;

            var sessionModel = sessionToUpdateData.Data as SessionModel;
            if (sessionModel == null)
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            // Update the properties of the session model with the new values from the form
            sessionModel.Title = form.Title;
            sessionModel.Description = form.Description;
            sessionModel.MaxParticipants = form.MaxParticipants;
            sessionModel.CurrentParticipants = form.CurrentParticipants;
            sessionModel.Date = form.Date;
            sessionModel.Intensity = form.Intensity;
            var updateResult = await _sessionService.UpdateSessionAsync(sessionModel);
            if (!updateResult.Success)
                return StatusCode(updateResult.StatusCode, new { message = updateResult.ResultMessage });
            return Ok(updateResult);
        } 
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var result = await _sessionService.DeleteSessionAsync(x => x.Id == id);

            if (!result.Success)
                return StatusCode(result.StatusCode, new { message = result.ResultMessage });

            return Ok();
        }
    }
}
