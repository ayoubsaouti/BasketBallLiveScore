using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Hub;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BasketBallLiveScore.Server.Services
{
    public class FoulService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MatchHub> _hubContext;

        public FoulService(ApplicationDbContext context, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Méthode pour enregistrer une faute
        public async Task<ActionResult> RecordFoulAsync(int matchId, FoulDTO foulDto)
        {
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .FirstOrDefaultAsync(m => m.MatchId == matchId);

            if (match == null)
            {
                return new NotFoundObjectResult("Match non trouvé.");
            }

            // Créer la faute avec les informations nécessaires
            var foul = new Foul
            {
                PlayerId = foulDto.PlayerId,
                FoulType = foulDto.FoulType,
                Time = DateTime.Now, // Convertir le temps (chaîne) en DateTime
                Quarter = foulDto.Quarter, // Associer la faute au quart-temps actuel
                ElapsedTime = foulDto.ElapsedTime // Utiliser la durée écoulée sur le timer
            };

            match.Fouls.Add(foul);

            // Enregistrer les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Envoyer un message à tous les clients via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveUpdate", match.MatchId, "foul", $"{foulDto.PlayerName} a commis une faute de type {foul.FoulType} au quart {foul.Quarter} à", foul.ElapsedTime);

            return new OkObjectResult(new { message = "Faute enregistrée avec succès" });
        }
    }
}
