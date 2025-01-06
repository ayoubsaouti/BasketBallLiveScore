using Microsoft.AspNetCore.Mvc;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Services;
using Microsoft.AspNetCore.Authorization;

namespace BasketBallLiveScore.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoulController : ControllerBase
    {
        private readonly FoulService _foulService;

        public FoulController(FoulService foulService)
        {
            _foulService = foulService;
        }

        // Endpoint pour enregistrer une faute
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/foul")]
        public async Task<ActionResult> RecordFoul(int id, [FromBody] FoulDTO foulDto)
        {
            var result = await _foulService.RecordFoulAsync(id, foulDto);

            return result;
        }
    }
}
