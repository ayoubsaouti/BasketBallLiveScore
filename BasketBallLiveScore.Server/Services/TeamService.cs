using BasketBallLiveScore.Server.Data;
using BasketBallLiveScore.Server.Models;


namespace BasketBallLiveScore.Server.Services
{
    public class TeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Rendre cette méthode asynchrone en ajoutant async et Task<T>
        public async Task<Team> CreateTeam(string name, string teamCode, string shortName, string teamColor)
        {
            var team = new Team
            {
                TeamName = name,
                TeamCode = teamCode,
                ShortName = shortName,
                TeamColor = teamColor
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();  // Utiliser la méthode asynchrone pour sauvegarder les données
            return team;
        }
    }
}
