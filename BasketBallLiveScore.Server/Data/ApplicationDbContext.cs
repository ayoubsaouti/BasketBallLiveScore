using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Models;
using TimeoutModel = BasketBallLiveScore.Server.Models.Timeout;

namespace BasketBallLiveScore.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets pour chaque entité
        public DbSet<Match> Matches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Quarter> Quarters { get; set; }
        public DbSet<User> Users { get; set; } // Table User
        public DbSet<Score> Scores { get; set; } // Table Score
        public DbSet<Foul> Fouls { get; set; } // Table Foul
        public DbSet<Substitution> Substitutions { get; set; } // Table Substitution
        public DbSet<TimeoutModel> Timeouts { get; set; } // Table Timeout

        // Configuration des relations et des contraintes
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration de la relation entre Match et Team (HomeTeam et AwayTeam)
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team1)
                .WithMany()
                .HasForeignKey(m => m.Team1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team2)
                .WithMany()
                .HasForeignKey(m => m.Team2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Relation entre Team et Player
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            // Configuration de la relation entre Match et Score
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Match)
                .WithMany(m => m.Scores)
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);  // En cas de suppression du match, les scores seront supprimés

            // Relation entre Score et Player
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Player)
                .WithMany()
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation entre Match et Foul
            modelBuilder.Entity<Foul>()
                .HasOne(f => f.Match)
                .WithMany(m => m.Fouls)
                .HasForeignKey(f => f.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre Foul et Player
            modelBuilder.Entity<Foul>()
                .HasOne(f => f.Player)
                .WithMany()
                .HasForeignKey(f => f.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation entre Match et Substitution
            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.Match)
                .WithMany(m => m.Substitutions)
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation entre Substitution et Player (Entrée et Sortie)
            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.SubInPlayer)
                .WithMany()
                .HasForeignKey(s => s.SubInPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Substitution>()
                .HasOne(s => s.SubOutPlayer)
                .WithMany()
                .HasForeignKey(s => s.SubOutPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation entre Match et Timeout
            modelBuilder.Entity<TimeoutModel>()
                .HasOne(t => t.Match)
                .WithMany(m => m.Timeouts)
                .HasForeignKey(t => t.MatchId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
