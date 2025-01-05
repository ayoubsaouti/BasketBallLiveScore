namespace BasketBallLiveScore.Server.DTO
{
    public class MatchFactsDTO
    {
        public int MatchId { get; set; }
        public string MatchNumber { get; set; }
        public string Competition { get; set; }
        public DateTime MatchDate { get; set; }
        public int Periods { get; set; }
        public int PeriodDuration { get; set; }
        public int OvertimeDuration { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public List<ScoreDTO> Scores { get; set; }  // Liste des scores
        public List<FoulDTO> Fouls { get; set; }    // Liste des fautes
    }

}
