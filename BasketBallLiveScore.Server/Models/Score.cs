namespace BasketBallLiveScore.Server.Models
{
    public class Score
    {
        public int ScoreId { get; set; }  // Identifiant unique du panier
        public int PlayerId { get; set; } // Identifiant du joueur qui a marqué
        public Player Player { get; set; } // Joueur qui a marqué
        public int Points { get; set; } // Nombre de points marqués (1, 2, 3)
        public int Quarter { get; set; } // Quart-temps du panier
        public DateTime Time { get; set; } // Heure à laquelle le panier a été marqué
        public int MatchId { get; set; } // Clé étrangère vers le match
        public Match Match { get; set; } // Propriété de navigation vers le match

    }
}
