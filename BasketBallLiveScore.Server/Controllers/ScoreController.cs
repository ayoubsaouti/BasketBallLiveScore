namespace BasketBallLiveScore.Server.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using BasketBallLiveScore.Server.Services;
    using BasketBallLiveScore.Server.DTO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly ScoreService _scoreService;

        public ScoreController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        // Endpoint pour enregistrer un score
        [Authorize(Roles = "Admin")]
        [HttpPost("{idMatch}/score")]
        public async Task<ActionResult> RecordScore(int idMatch, [FromBody] ScoreDTO scoreDto)
        {
            // Appeler le service ScoreService pour enregistrer un score
            var result = await _scoreService.RecordScoreAsync(idMatch, scoreDto);

            return result; // Le résultat retourné par le service est déjà un ActionResult
        }
    }
}
