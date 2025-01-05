namespace BasketBallLiveScore.Server.Models
{
    public class Substitution
    {
        public int SubstitutionId { get; set; } // Identifiant de la substitution
        public int SubInPlayerId { get; set; } // Identifiant du joueur entrant
        public Player SubInPlayer { get; set; } // Joueur entrant
        public int SubOutPlayerId { get; set; } // Identifiant du joueur sortant
        public Player SubOutPlayer { get; set; } // Joueur sortant
        public int Quarter { get; set; } // Quart-temps du changement
        public TimeSpan Time { get; set; } // Heure à laquelle le changement a eu lieu
        public int MatchId { get; set; } // Clé étrangère vers le match
        public Match Match { get; set; } // Propriété de navigation vers le match
    }
}
