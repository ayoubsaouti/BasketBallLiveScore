namespace BasketBallLiveScore.Server.DTO
{
    public class ConfigMatchDTO
    {
        public string MatchNumber { get; set; }  // Numéro du match
        public string Competition { get; set; }  // Nom de la compétition
        public DateTime MatchDate { get; set; }  // Date et heure du match
        public int Periods { get; set; }  // Nombre de périodes
        public int PeriodDuration { get; set; }  // Durée de chaque période (en minutes)
        public int OvertimeDuration { get; set; }  // Durée du prolongement (en minutes)

    }

}
