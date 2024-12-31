namespace BasketBallLiveScore.Server.Models
{
    public class Quarter
    {
        public int QuarterId { get; set; }  // Clé primaire
        public int MatchId { get; set; }    // Clé étrangère vers Match
        public Match Match { get; set; }    // Propriété de navigation vers Match

        public int QuarterNumber { get; set; }  // nbr de quart (1, 2, 3, 4)
        public TimeSpan QuarterDuration { get; set; }  // Durée du quart
    }
}
