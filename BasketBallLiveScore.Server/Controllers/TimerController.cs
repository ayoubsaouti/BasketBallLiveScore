namespace BasketBallLiveScore.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using BasketBallLiveScore.Server.Services;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/[controller]")]
    public class TimerController : ControllerBase
    {
        private readonly TimerService _timerService;

        public TimerController(TimerService timerService)
        {
            _timerService = timerService;
        }

        // Endpoint pour mettre à jour le timer
        [Authorize(Roles = "Admin")]
        [HttpPost("updateElapsedTimer/{matchId}")]
        public async Task<ActionResult> UpdateElapsedTimer(int matchId, [FromBody] int elapsedTimer)
        {
            // Appeler le service TimerService pour mettre à jour le timer
            var result = await _timerService.UpdateElapsedTimerAsync(matchId, elapsedTimer);

            return result; // Le résultat retourné par le service est déjà un ActionResult
        }

        // Endpoint pour terminer le match
        [Authorize(Roles = "Admin")]
        [HttpPost("finishMatch/{matchId}")]
        public async Task<IActionResult> FinishMatch(int matchId)
        {
            // Appeler le service TimerService pour terminer le match
            var result = await _timerService.FinishMatchAsync(matchId);

            return result; // Le résultat retourné par le service est déjà un IActionResult
        }
    }
}
