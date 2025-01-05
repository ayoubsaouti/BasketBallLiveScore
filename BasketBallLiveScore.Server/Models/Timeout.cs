namespace BasketBallLiveScore.Server.Models
{
    public class Timeout
    {
        public int TimeoutId { get; set; } // Identifiant du time-out
        public string Team { get; set; } // L'équipe qui a demandé le time-out (home ou away)
        public int Quarter { get; set; } // Quart-temps du time-out
        public TimeSpan Time { get; set; } // Heure à laquelle le time-out a été demandé
        public int MatchId { get; set; } // Clé étrangère vers le match
        public Match Match { get; set; } // Propriété de navigation vers le match
    }
}
