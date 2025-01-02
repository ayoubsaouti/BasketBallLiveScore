namespace BasketBallLiveScore.Server.DTO
{
    public class SubstitutionDTO
    {
        public int SubInPlayerId { get; set; }
        public int SubOutPlayerId { get; set; }
        public int Quarter { get; set; } // Le quart-temps
        public string Time { get; set; } // Le temps du match
    }

}
