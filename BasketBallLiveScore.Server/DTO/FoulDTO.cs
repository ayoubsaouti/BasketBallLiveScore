namespace BasketBallLiveScore.Server.DTO
{
    public class FoulDTO
    {
        public int PlayerId { get; set; }
        public string FoulType { get; set; }
        public int Quarter { get; set; }
        public DateTime Time { get; set; }
        public string PlayerName { get; set; }  // Nom du joueur
        public double ElapsedTime { get; set; } // Temps écoulé en secondes

    }
}
