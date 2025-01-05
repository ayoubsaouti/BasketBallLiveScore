

namespace BasketBallLiveScore.Server.Models
{
    public class Team
    {
        public int TeamId { get; set; }       // Identifiant unique de l'équipe
        public string TeamName { get; set; }  // Nom de l'équipe
        public string TeamCode { get; set; }

        public string ShortName { get; set; } // Nom court de l'équipe
        public string TeamColor { get; set; } // Couleur de l'équipe

        public ICollection<Player> Players { get; set; } = new List<Player>();  // Initialisation de la collection

    }
}

