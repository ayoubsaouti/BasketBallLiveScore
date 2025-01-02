using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

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
            var matches = await _context.Matches
                    .Include(m => m.Team1)
                    .ThenInclude(t => t.Players)
                    .Include(m => m.Team2)
                    .ThenInclude(t => t.Players)
                    .Select(m => new MatchDTO
                    {
                        MatchId = m.MatchId,
                        MatchNumber = m.MatchNumber,
                        Competition = m.Competition,
                        MatchDate = m.MatchDate,
                        Periods = m.Periods,
                        PeriodDuration = m.PeriodDuration,
                        OvertimeDuration = m.OvertimeDuration,
                        HomeTeamName = m.Team1.TeamName,
                        AwayTeamName = m.Team2.TeamName,
                        HomePlayers = m.Team1.Players.Select(p => new PlayerDTO
                        {
                            PlayerId = p.PlayerId,
                            Number = p.Number,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Position = p.Position,
                            IsCaptain = p.IsCaptain,
                            IsInGame = p.IsInGame
                        }).ToList(),
                        AwayPlayers = m.Team2.Players.Select(p => new PlayerDTO
                        {
                            PlayerId = p.PlayerId,
                            Number = p.Number,
                            FirstName = p.FirstName,
                            LastName = p.LastName,
                            Position = p.Position,
                            IsCaptain = p.IsCaptain,
                            IsInGame = p.IsInGame
                        }).ToList()
                    })
                    .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return NotFound("Aucun match trouvé.");
            }

            return Ok(matches);
        }


        [HttpGet("getMatch/{matchId}")]
        public async Task<ActionResult> GetMatchById(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Team1)  // Inclure les informations de l'équipe 1
                .ThenInclude(t => t.Players)  // Inclure les joueurs de l'équipe 1
                .Include(m => m.Team2)  // Inclure les informations de l'équipe 2
                .ThenInclude(t => t.Players)  // Inclure les joueurs de l'équipe 2
                .FirstOrDefaultAsync(m => m.MatchId == matchId);  // Récupérer le match par son ID

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Mapper les données du match dans un DTO (Data Transfer Object)
            var matchDTO = new MatchDTO
            {
                MatchId = match.MatchId,
                MatchNumber = match.MatchNumber,
                Competition = match.Competition,
                MatchDate = match.MatchDate,
                Periods = match.Periods,
                PeriodDuration = match.PeriodDuration,
                OvertimeDuration = match.OvertimeDuration,
                HomeTeamName = match.Team1.TeamName,
                AwayTeamName = match.Team2.TeamName,
                HomePlayers = match.Team1.Players.Select(p => new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    Number = p.Number,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Position = p.Position,
                    IsCaptain = p.IsCaptain,
                    IsInGame = p.IsInGame
                }).ToList(),
                AwayPlayers = match.Team2.Players.Select(p => new PlayerDTO
                {
                    PlayerId = p.PlayerId,
                    Number = p.Number,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Position = p.Position,
                    IsCaptain = p.IsCaptain,
                    IsInGame = p.IsInGame
                }).ToList()
            };

            return Ok(matchDTO);  // Retourner les détails du match
        }

        // Endpoint pour enregistrer un panier marqué
        [HttpPost("{id}/score")]
        public async Task<ActionResult> RecordScore(int id, [FromBody] ScoreDto scoreDto)
        {
            var match = await _context.Matches.Include(m => m.HomeTeam).Include(m => m.AwayTeam).FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }

            // Ajouter le score (logique métier)
            var score = new Score
            {
                PlayerId = scoreDto.PlayerId,
                Points = scoreDto.Points,
                Time = DateTime.Now
            };

            match.Scores.Add(score);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Panier marqué avec succès" });
        }

        // Endpoint pour enregistrer une faute
        [HttpPost("{id}/foul")]
        public async Task<ActionResult> RecordFoul(int id, [FromBody] FoulDto foulDto)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }

            var foul = new Foul
            {
                PlayerId = foulDto.PlayerId,
                FoulType = foulDto.FoulType,
                Time = DateTime.Now,
                Quarter = foulDto.Quarter
            };

            match.Fouls.Add(foul);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Faute enregistrée avec succès" });
        }

        [HttpPost("{id}/substitution")]
        public async Task<ActionResult> RecordSubstitution(int id, [FromBody] SubstitutionDto substitutionDto)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }

            // Vérifier si les joueurs existent dans le match
            var subInPlayer = await _context.Players.FindAsync(substitutionDto.SubInPlayerId);
            var subOutPlayer = await _context.Players.FindAsync(substitutionDto.SubOutPlayerId);

            if (subInPlayer == null || subOutPlayer == null)
            {
                return BadRequest("Joueur entrant ou sortant invalide.");
            }

            // Créer une entrée de substitution
            var substitution = new Substitution
            {
                SubInPlayerId = substitutionDto.SubInPlayerId,
                SubOutPlayerId = substitutionDto.SubOutPlayerId,
                Time = DateTime.Now,  // Le moment exact de la substitution
                Quarter = substitutionDto.Quarter
            };

            // Ajouter la substitution à l'historique
            _context.Substitutions.Add(substitution);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Changement de joueur enregistré avec succès" });
        }


        [HttpPost("{id}/timeout")]
        public async Task<ActionResult> RecordTimeout(int id, [FromBody] TimeoutDto timeoutDto)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }

            // Enregistrer le time-out de l'équipe
            var timeout = new Timeout
            {
                Team = timeoutDto.Team,  // 'home' ou 'away'
                Quarter = timeoutDto.Quarter,
                Time = DateTime.Now  // Le moment exact du time-out
            };

            _context.Timeouts.Add(timeout);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Time-out enregistré avec succès" });
        }


    }
}
