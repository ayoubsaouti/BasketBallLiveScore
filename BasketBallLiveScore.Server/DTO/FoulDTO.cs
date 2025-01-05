namespace BasketBallLiveScore.Server.DTO
{
    public class FoulDTO
    {
        public int PlayerId { get; set; }  // Identifiant du joueur
        public string FoulType { get; set; }  // Type de faute (P0, P1, P2, etc.)
        public int Quarter { get; set; }  // Quart-temps dans lequel la faute a été commise
        public string Time { get; set; }  // Heure à laquelle la faute a été commise (sous forme de chaîne)
    }
}
