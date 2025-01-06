using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.Hub;
using BasketBallLiveScore.Server.DTO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasketBallLiveScore.Server.Services
{
    public class ScoreService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MatchHub> _hubContext;

        public ScoreService(ApplicationDbContext context, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ActionResult> RecordScoreAsync(int matchId, ScoreDTO scoreDto)
        {
            var match = await _context.Matches
                .Include(m => m.Team1).ThenInclude(t => t.Players)
                .Include(m => m.Team2).ThenInclude(t => t.Players)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return new NotFoundObjectResult("Match non trouvé.");
            }

            // Vérifier si le joueur existe dans l'une des équipes
            var player = match.Team1.Players.Concat(match.Team2.Players)
                .FirstOrDefault(p => p.PlayerId == scoreDto.PlayerId);

            if (player == null)
            {
                return new BadRequestObjectResult("Le joueur n'existe pas dans ce match.");
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

            return new OkObjectResult(new { message = "Panier marqué avec succès" });
        }
    }
}
