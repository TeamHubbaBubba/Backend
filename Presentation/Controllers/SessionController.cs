using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class SessionController(ISessionService sessionService) : Controller
    {
        private readonly ISessionService _sessionService = sessionService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var sessions = await _sessionService.GetAllSessionsAsync();

            return Ok(sessions);
        }
    }
}
