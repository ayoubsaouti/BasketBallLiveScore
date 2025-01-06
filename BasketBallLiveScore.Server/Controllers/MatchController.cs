using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Services;

namespace BasketBallLiveScore.Server.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TeamService _teamService;
        private readonly MatchService _matchService;


        public MatchController(ApplicationDbContext context, TeamService teamService, MatchService matchService)
        {
            _context = context;
            _teamService = teamService;
            _matchService = matchService;

        }


        // Étape 1: Configurer le match avec les données de base
        [HttpPost("newMatch")]
        public async Task<ActionResult> ConfigureMatch([FromBody] ConfigMatchDTO config)
        {
            if (config == null)
            {
                return BadRequest("Les données du match sont invalides.");
            }

            

            // Créer le match avec les informations de base (sans les équipes)
            var match = _matchService.CreateMatch(
                config.MatchNumber,
                config.Competition,
                config.MatchDate,
                config.Periods,
                config.PeriodDuration,
                config.OvertimeDuration,
                "Test", // Enregistre l'email de l'utilisateur qui crée le match
                null, // Pas d'équipes associées pour l'instant
                null  // Pas d'équipes associées pour l'instant
            );

            return Ok(new { message = "Match configuré avec succès.", matchId = match.MatchId });
        }

        // Étape 2: Ajouter les équipes au match sans ajouter les joueurs
        [HttpPost("addTeams/{matchId}")]
        public async Task<ActionResult> AddTeamsToMatch(int matchId, [FromBody] TeamDetailsDTO teams)
        {
            // Récupérer le match
            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Créer ou récupérer l'équipe à domicile
            var homeTeam = await _teamService.CreateTeam(teams.HomeTeamName, teams.HomeTeamCode, teams.HomeShortName, teams.HomeTeamColor);

            // Créer ou récupérer l'équipe visiteuse
            var awayTeam = await _teamService.CreateTeam(teams.AwayTeamName, teams.AwayTeamCode, teams.AwayShortName, teams.AwayTeamColor);

            // Associer les équipes au match
            match.Team1 = homeTeam;
            match.Team1Id = homeTeam.TeamId;

            match.Team2 = awayTeam;
            match.Team2Id = awayTeam.TeamId;

            // Sauvegarder les modifications (match et équipes)
            await _context.SaveChangesAsync();

            return Ok(new { message = "Équipes ajoutées au match avec succès." });
        }


        // Ajouter les joueurs aux équipes du match
        [HttpPost("addPlayers/{matchId}")]
        public async Task<ActionResult> AddPlayersToMatch(int matchId, [FromBody] PlayerDetailsDTO playerDetails)
        {
            var result = await _matchService.AddPlayersToMatchAsync(matchId, playerDetails);

            if (result)
            {
                return Ok(new { message = "Joueurs ajoutés aux équipes du match avec succès." });
            }

            return NotFound("Match non trouvé.");
        }

        [HttpGet("getMatches")]
        public async Task<ActionResult> GetMatches()
        {
            // Appeler la méthode du service pour récupérer les matchs
            var matches = await _matchService.GetAllMatchesAsync();

            if (matches == null || matches.Count == 0)
            {
                return NotFound("Aucun match trouvé.");
            }

            return Ok(matches);
        }

        //ENDPOINT pour récuperer toutes les infos d'un match (regle)
        [HttpGet("getMatch/{matchId}")]
        public async Task<ActionResult> GetMatchById(int matchId)
        {
            var match = await _matchService.GetMatchByIdAsync(matchId);

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            return Ok(match); // Retourner les détails du match
        }

        //ENDPOINT pour récuperer les faits d'un match
        [HttpGet("getFacts/{matchId}")]
        public async Task<ActionResult> GetMatchFacts(int matchId)
        {
            var matchFacts = await _matchService.GetMatchFactsAsync(matchId);

            if (matchFacts == null)
            {
                return NotFound("Match non trouvé.");
            }

            return Ok(matchFacts); // Retourner les faits du match
        }

        //ENDPOINT pour update le quarter actuelle
        [HttpPost("updateQuarter/{matchId}")]
        public async Task<IActionResult> UpdateQuarter(int matchId)
        {
            var result = await _matchService.UpdateQuarterAsync(matchId);

            return result;
        }
    }
}
