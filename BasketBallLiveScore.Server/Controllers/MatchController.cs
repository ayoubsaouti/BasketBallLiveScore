using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Services;
using System.Threading.Tasks;

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
            // Récupérer le match
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Ajouter les joueurs à l'équipe à domicile
            foreach (var player in playerDetails.HomePlayers)
            {
                var newPlayer = new Player
                {
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    Number = player.Number,
                    Position = player.Position,
                    IsCaptain = player.IsCaptain,
                    IsInGame = player.IsInGame
                };
                match.Team1.Players.Add(newPlayer);
            }

            // Ajouter les joueurs à l'équipe visiteuse
            foreach (var player in playerDetails.AwayPlayers)
            {
                var newPlayer = new Player
                {
                    FirstName = player.FirstName,
                    LastName = player.LastName,
                    Number = player.Number,
                    Position = player.Position,
                    IsCaptain = player.IsCaptain,
                    IsInGame = player.IsInGame
                };
                match.Team2.Players.Add(newPlayer);
            }

            // Sauvegarder les modifications (ajout des joueurs)
            await _context.SaveChangesAsync();

            return Ok(new { message = "Joueurs ajoutés aux équipes du match avec succès." });
        }


        [HttpGet("getMatches")]
        public async Task<ActionResult> GetMatches()
        {
            // Récupérer tous les matchs avec leurs équipes associées
            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return NotFound("Aucun match trouvé.");
            }

            return Ok(matches);
        }



    }
}
