namespace BasketBallLiveScore.Server.DTO
{
    public class FoulDTO
    {
        public int PlayerId { get; set; }
        public string FoulType { get; set; }    
        public int Quarter { get; set; }
        public string Time { get; set; }
    }
}
