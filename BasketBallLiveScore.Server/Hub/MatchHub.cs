namespace BasketBallLiveScore.Server.Hub
{
    using Microsoft.AspNetCore.SignalR;

    public class MatchHub : Hub
    {
        // Méthode pour envoyer une mise à jour à tous les clients connectés
        public async Task UpdateMatch(int matchId, string actionType, string message, int ElapsedTime)
        {
            // Envoie un message à tous les clients connectés
            await Clients.All.SendAsync("ReceiveUpdate", matchId, actionType, message, ElapsedTime);
        }

        // Méthode pour envoyer une mise à jour des scores à tous les clients connectés
        public async Task SendScoreUpdate(int matchId, int homeTeamScore, int awayTeamScore)
        {
            // Envoi de la mise à jour des scores à tous les clients connectés
            await Clients.All.SendAsync("ReceiveScoreUpdate", matchId, homeTeamScore, awayTeamScore);
        }

        // Méthode pour envoyer une mise à jour du timer à tous les clients connectés
        public async Task SendTimerUpdate(int matchId, int elapsedTime)
        {
            // Envoie de la mise à jour du timer
            await Clients.All.SendAsync("ReceiveTimerUpdate", matchId, elapsedTime);
        }
        // Méthode pour notifier tous les clients que le match est terminé
        public async Task SendMatchFinishedUpdate(int matchId, bool isFinished)
        {
            await Clients.All.SendAsync("ReceiveMatchFinished", matchId, isFinished);
        }

        // Méthode pour envoyer une mise à jour du quart actuel
        public async Task UpdateQuarter(int matchId, int currentQuarter)
        {
            // Envoi de la mise à jour du quart actuel à tous les clients connectés
            await Clients.All.SendAsync("ReceiveQuarterUpdate", matchId, currentQuarter);
        }
    }
}
