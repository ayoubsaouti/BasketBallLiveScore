namespace BasketBallLiveScore.Server.Models
{
    public class Match
    {
        public int MatchId { get; set; }           // Identifiant unique du match
        public string MatchNumber { get; set; }    // Numéro du match
        public string Competition { get; set; }    // Compétition
        public DateTime MatchDate { get; set; }    // Date et heure du match
        public int Periods { get; set; }           // Nombre de périodes
        public int PeriodDuration { get; set; }    // Durée de chaque période en minutes
        public int OvertimeDuration { get; set; }  // Durée du prolongation en minutes
        public string Encoder { get; set; }


        // Clés étrangères pour les équipes
        public int? Team1Id { get; set; }           // ID de l'équipe à domicile
        public Team? Team1 { get; set; }            // L'équipe à domicile

        public int? Team2Id { get; set; }           // ID de l'équipe visiteuse
        public Team? Team2 { get; set; }            // L'équipe visiteuse

    }

}
