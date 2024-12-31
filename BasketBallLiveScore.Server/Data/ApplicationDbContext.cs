using Microsoft.EntityFrameworkCore;
using BasketBallLiveScore.Server.Models;

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
        public DbSet<User> Users { get; set; } // Ajout de la table User

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
        }
    }
}
