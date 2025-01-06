namespace BasketBallLiveScore.Server.Models
{
    public class Foul
    {
        public int FoulId { get; set; } // Identifiant de la faute
        public int PlayerId { get; set; } // Identifiant du joueur fautif
        public Player Player { get; set; } // Joueur fautif
        public string FoulType { get; set; } // Type de faute (P0, P1, P2, P3)
        public int Quarter { get; set; } // Quart-temps de la faute
        public DateTime Time { get; set; } // Heure à laquelle la faute a été commise
        public int MatchId { get; set; } // Clé étrangère vers le match
        public Match Match { get; set; } // Propriété de navigation vers le match
        public double ElapsedTime { get; set; } // Durée écoulée sur le timer (en secondes)

    }
}
