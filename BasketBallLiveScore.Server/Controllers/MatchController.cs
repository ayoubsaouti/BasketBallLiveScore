using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BasketBallLiveScore.Server.Hub;
using Microsoft.AspNetCore.SignalR;

namespace BasketBallLiveScore.Server.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TeamService _teamService;
        private readonly MatchService _matchService;
        private readonly IHubContext<MatchHub> _hubContext;


        public MatchController(ApplicationDbContext context, TeamService teamService, MatchService matchService, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _teamService = teamService;
            _matchService = matchService;
            _hubContext = hubContext;

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
                ElapsedTimer = match.ElapsedTimer,
                IsFinished = match.IsFinished,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                CurrentQuarter = match.CurrentQuarter,
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

        [HttpGet("getFacts/{matchId}")]
        public async Task<ActionResult> GetMatchFacts(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Team1) // Inclure l'équipe 1
                .Include(m => m.Team2) // Inclure l'équipe 2
                .Include(m => m.Scores) // Inclure les scores
                    .ThenInclude(s => s.Player) // Inclure les joueurs associés au score
                .Include(m => m.Fouls)  // Inclure les fautes
                    .ThenInclude(f => f.Player) // Inclure les joueurs associés à la faute
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Créer un objet DTO pour les faits du match (MatchFactsDTO)
            var matchFactsDTO = new MatchFactsDTO
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
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                Scores = match.Scores.Select(s => new ScoreDTO
                {
                    PlayerId = s.PlayerId,
                    Points = s.Points,
                    Quarter = s.Quarter,
                    Time = s.Time,
                    PlayerName = s.Player != null ? $"{s.Player.FirstName} {s.Player.LastName}" : "Joueur Inconnu",// Vérification si Player est null
                    ElapsedTime = s.ElapsedTime
                }).ToList(),
                Fouls = match.Fouls.Select(f => new FoulDTO
                {
                    PlayerId = f.PlayerId,
                    FoulType = f.FoulType,
                    Quarter = f.Quarter,
                    Time = f.Time,
                    PlayerName = f.Player != null ? $"{f.Player.FirstName} {f.Player.LastName}" : "Joueur Inconnu", // Vérification si Player est null
                    ElapsedTime = f.ElapsedTime
                }).ToList()
            };

            return Ok(matchFactsDTO);  // Retourner les faits du match
        }

        [HttpPost("{idMatch}/score")]
        public async Task<ActionResult> RecordScore(int idMatch, [FromBody] ScoreDTO scoreDto)
        {
            var match = await _context.Matches
                .Include(m => m.Team1).ThenInclude(t => t.Players)
                .Include(m => m.Team2).ThenInclude(t => t.Players)
                .FirstOrDefaultAsync(m => m.MatchId == idMatch);

            if (match == null)
            {
                return NotFound();
            }

            // Vérifier si le joueur existe dans l'une des équipes
            var player = match.Team1.Players.Concat(match.Team2.Players)
                .FirstOrDefault(p => p.PlayerId == scoreDto.PlayerId);

            if (player == null)
            {
                return BadRequest("Le joueur n'existe pas dans ce match.");
            }

            // Ajouter le score avec l'heure du timer
            var score = new Score
            {
                PlayerId = scoreDto.PlayerId,
                Points = scoreDto.Points,
                Player = player,
                Quarter = scoreDto.Quarter, 
                Time = DateTime.Now, // Enregistrer l'heure actuelle
                ElapsedTime = scoreDto.ElapsedTime, // Utiliser la durée écoulée sur le timer
                MatchId = match.MatchId // Associer le score au match
            };

            match.Scores.Add(score);

            // Incrémenter le score de l'équipe en fonction du joueur
            if (player.TeamId == match.Team1Id)
            {
                match.HomeTeamScore += scoreDto.Points; // Incrémenter le score de l'équipe à domicile
            }
            else if (player.TeamId == match.Team2Id)
            {
                match.AwayTeamScore += scoreDto.Points; // Incrémenter le score de l'équipe visiteuse
            }
            await _context.SaveChangesAsync();

            // Appeler SignalR pour notifier tous les clients du nouveau score
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", match.MatchId, "score", $"{score.Player.FirstName} {score.Player.LastName} a marqué {score.Points} points au quart {score.Quarter} à ", score.ElapsedTime);
            
            // Appeler SignalR pour notifier tous les clients du nouveau score
            await _hubContext.Clients.All.SendAsync("ReceiveScoreUpdate", match.MatchId, match.HomeTeamScore, match.AwayTeamScore);


            return Ok(new { message = "Panier marqué avec succès" });
        }




        // Endpoint pour enregistrer une faute
        [HttpPost("{id}/foul")]
        public async Task<ActionResult> RecordFoul(int id, [FromBody] FoulDTO foulDto)
        {
            // Récupérer le match par son ID, incluant les informations des équipes
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .FirstOrDefaultAsync(m => m.MatchId == id);

            if (match == null)
            {
                return NotFound();
            }


            // Créer la faute avec les informations nécessaires
            var foul = new Foul
            {
                PlayerId = foulDto.PlayerId,
                FoulType = foulDto.FoulType,
                Time = DateTime.Now, // Convertir le temps (chaîne) en DateTime
                Quarter = foulDto.Quarter, // Associer la faute au quart-temps actuel
                ElapsedTime = foulDto.ElapsedTime, // Utiliser la durée écoulée sur le timer

            };

            match.Fouls.Add(foul);

            // Enregistrer les modifications dans la base de données
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", match.MatchId, "foul", $"{foulDto.PlayerName} a commis une faute de type {foul.FoulType} au quart {foul.Quarter} à ", foul.ElapsedTime);


            return Ok(new { message = "Faute enregistrée avec succès" });
        }

        [HttpPost("updateElapsedTimer/{matchId}")]
        public async Task<ActionResult> UpdateElapsedTimer(int matchId, [FromBody] int elapsedTimer)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Mettre à jour le temps du timer
            match.ElapsedTimer = elapsedTimer;

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Appeler SignalR pour envoyer la mise à jour du timer à tous les clients connectés
            await _hubContext.Clients.All.SendAsync("ReceiveTimerUpdate", matchId, elapsedTimer);

            return Ok(new { message = "Temps du match mis à jour avec succès", elapsedTimer });
        }

        [HttpPost("finishMatch/{matchId}")]
        public async Task<IActionResult> FinishMatch(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            match.IsFinished = true; // Ajouter un champ "IsFinished" à votre modèle Match pour suivre l'état du match

            await _context.SaveChangesAsync();

            // Appeler SignalR pour notifier tous les clients que le match est terminé
            await _hubContext.Clients.All.SendAsync("ReceiveMatchFinished", match.MatchId, match.IsFinished);

            return Ok(new { message = "Match marqué comme terminé" });
        }


        [HttpPost("updateQuarter/{matchId}")]
        public async Task<IActionResult> UpdateQuarter(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Scores) // Inclure les scores du match
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return NotFound("Match non trouvé.");
            }

            // Vérifier si le match est terminé
            if (match.Periods == match.CurrentQuarter)
            {
                // Si les quarts sont terminés, marquer le match comme terminé
                match.IsFinished = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Le match est terminé.", isFinished = true });
            }

            // Passer au quart suivant
            match.CurrentQuarter += 1;

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Mettre à jour le quart actuel via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveQuarterUpdate", match.MatchId, match.CurrentQuarter);


            return Ok(new { message = $"Passage au quart {match.CurrentQuarter}", currentQuarter = match.CurrentQuarter });
        }

        
    }
}
