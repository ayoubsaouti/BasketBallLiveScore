using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;
using BasketBallLiveScore.Server.Hub;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BasketBallLiveScore.Server.Services
{
    public class TimerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<MatchHub> _hubContext;

        public TimerService(ApplicationDbContext context, IHubContext<MatchHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ActionResult> UpdateElapsedTimerAsync(int matchId, int elapsedTimer)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            if (match == null)
            {
                return new NotFoundObjectResult("Match non trouvé.");
            }

            // Mettre à jour le temps du timer
            match.ElapsedTimer = elapsedTimer;

            // Sauvegarder les modifications dans la base de données
            await _context.SaveChangesAsync();

            // Appeler SignalR pour envoyer la mise à jour du timer à tous les clients connectés
            await _hubContext.Clients.All.SendAsync("ReceiveTimerUpdate", matchId, elapsedTimer);

            return new OkObjectResult(new { message = "Temps du match mis à jour avec succès", elapsedTimer });
        }

        public async Task<IActionResult> FinishMatchAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.MatchId == matchId);
            if (match == null)
            {
                return new NotFoundObjectResult("Match non trouvé.");
            }

            match.IsFinished = true; // Ajouter un champ "IsFinished" à votre modèle Match pour suivre l'état du match

            await _context.SaveChangesAsync();

            // Appeler SignalR pour notifier tous les clients que le match est terminé
            await _hubContext.Clients.All.SendAsync("ReceiveMatchFinished", match.MatchId, match.IsFinished);

            return new OkObjectResult(new { message = "Match marqué comme terminé" });
        }
    }
}
