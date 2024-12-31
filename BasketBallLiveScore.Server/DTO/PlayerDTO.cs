namespace BasketBallLiveScore.Server.DTO
{
    public class PlayerDTO
    {
        public int Number { get; set; }          // Numéro du joueur
        public string FirstName { get; set; }    // Prénom du joueur
        public string LastName { get; set; }     // Nom du joueur
        public string Position { get; set; }     // Position du joueur
        public bool IsCaptain { get; set; }      // Indique si c'est le capitaine
        public bool IsInGame { get; set; }       // Indique si le joueur est actuellement en jeu
    }

}
