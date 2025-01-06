using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.DTO;
using BasketBallLiveScore.Server.Hub;
using BasketBallLiveScore.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BasketBallLiveScore.Server.Services
{
    public class MatchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MatchHub> _hubContext;


        public MatchService(ApplicationDbContext context, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;

        }

        // Creer le match de base
        public Match CreateMatch(string matchNumber, string competition, DateTime matchDate, int numberOfPeriods, int periodDuration, int overtimeDuration,String encoder, Team team1, Team team2)
        {
            var match = new Match
            {
                MatchNumber = matchNumber,
                Competition = competition,
                MatchDate = matchDate,
                Periods = numberOfPeriods,
                PeriodDuration = periodDuration,
                OvertimeDuration = overtimeDuration,
                Encoder = encoder,
                Team1 = team1,
                Team2 = team2
            };

            _context.Matches.Add(match);
            _context.SaveChanges();
            return match;
        }



        // Ajouter des joueurs aux équipes du match
        public async Task<bool> AddPlayersToMatchAsync(int matchId, PlayerDetailsDTO playerDetails)
        {
            // Récupérer le match
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return false;
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

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            return true;
        }


        // Méthode pour récupérer tous les matchs
        public async Task<List<MatchDTO>> GetAllMatchesAsync()
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

            return matches;
        }

        // Méthode pour récupérer les détails d'un match par son ID
        public async Task<MatchDTO> GetMatchByIdAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Team1)
                    .ThenInclude(t => t.Players)
                .Include(m => m.Team2)
                    .ThenInclude(t => t.Players)
                .FirstOrDefaultAsync(m => m.MatchId == matchId); // Récupérer le match par son ID

            if (match == null)
            {
                return null; // Si le match n'est pas trouvé, retourner null
            }

            // Mapper les données du match dans un DTO
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

            return matchDTO;
        }

        // Méthode pour récupérer les faits d'un match
        public async Task<MatchFactsDTO> GetMatchFactsAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Include(m => m.Scores)
                    .ThenInclude(s => s.Player)
                .Include(m => m.Fouls)
                    .ThenInclude(f => f.Player)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return null; // Retourner null si le match n'est pas trouvé
            }

            // Mapper les données du match dans un DTO pour les faits du match
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
                    PlayerName = s.Player != null ? $"{s.Player.FirstName} {s.Player.LastName}" : "Joueur Inconnu",
                    ElapsedTime = s.ElapsedTime
                }).ToList(),
                Fouls = match.Fouls.Select(f => new FoulDTO
                {
                    PlayerId = f.PlayerId,
                    FoulType = f.FoulType,
                    Quarter = f.Quarter,
                    Time = f.Time,
                    PlayerName = f.Player != null ? $"{f.Player.FirstName} {f.Player.LastName}" : "Joueur Inconnu",
                    ElapsedTime = f.ElapsedTime
                }).ToList()
            };

            return matchFactsDTO; // Retourner les faits du match
        }

        // Méthode pour mettre à jour le quart-temps
        public async Task<IActionResult> UpdateQuarterAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Scores) // Inclure les scores du match
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return new NotFoundObjectResult("Match non trouvé.");
            }

            // Vérifier si le match est terminé
            if (match.Periods == match.CurrentQuarter)
            {
                // Si les quarts sont terminés, marquer le match comme terminé
                match.IsFinished = true;
                await _context.SaveChangesAsync();

                // Informer tous les clients via SignalR que le match est terminé
                await _hubContext.Clients.All.SendAsync("ReceiveMatchFinished", match.MatchId, match.IsFinished);

                return new OkObjectResult(new { message = "Le match est terminé.", isFinished = true });
            }

            // Passer au quart suivant
            match.CurrentQuarter += 1;

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Mettre à jour le quart actuel via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveQuarterUpdate", match.MatchId, match.CurrentQuarter);

            return new OkObjectResult(new { message = $"Passage au quart {match.CurrentQuarter}", currentQuarter = match.CurrentQuarter });
        }

    }
}
