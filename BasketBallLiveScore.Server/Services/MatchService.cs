using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;

namespace BasketBallLiveScore.Server.Services
{
    public class MatchService
    {
        private readonly ApplicationDbContext _context;

        public MatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Match CreateMatch(string matchNumber, string competition, DateTime matchDate, int numberOfPeriods, int periodDuration, int overtimeDuration, Team team1, Team team2)
        {
            var match = new Match
            {
                MatchNumber = matchNumber,
                Competition = competition,
                MatchDate = matchDate,
                Periods = numberOfPeriods,
                PeriodDuration = periodDuration,
                OvertimeDuration = overtimeDuration,
                Team1 = team1,
                Team2 = team2
            };

            _context.Matches.Add(match);
            _context.SaveChanges();
            return match;
        }
    }

}
