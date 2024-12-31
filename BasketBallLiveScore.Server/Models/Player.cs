namespace BasketBallLiveScore.Server.Models
{

    public class Player
    {
        public int PlayerId { get; set; }       // Identifiant unique du joueur
        public int Number { get; set; }         // Numéro du joueur
        public string FirstName { get; set; }   // Prénom du joueur
        public string LastName { get; set; }    // Nom du joueur
        public string Position { get; set; }    // Position du joueur
        public bool IsCaptain { get; set; }     // Si c'est le capitaine
        public bool IsInGame { get; set; }      // Si le joueur est actuellement en jeu

        // Clé étrangère vers l'équipe
        public int TeamId { get; set; }         // ID de l'équipe
        public Team Team { get; set; }          // L'équipe du joueur
    }

}
