using Microsoft.AspNetCore.Mvc;
using BasketBallLiveScore.Server.Services;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Models;

namespace BasketBallLiveScore.Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Enregistrement d'un utilisateur
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                var user = await _authService.RegisterAsync(request);
                return Ok(user);  // Envoie l'utilisateur créé
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Connexion d'un utilisateur
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var loginResponse = await _authService.LoginAsync(request);
                return Ok(loginResponse);  // Envoie le token dans la réponse
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
