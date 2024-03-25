using BasketBet.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasketBet.EntityFramework.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Bet> Bets { get; set; }
        public DbSet<BetItem> BetItems { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Game>()
                .HasOne(g => g.HomeTeam)
                .WithMany(t => t.HomeGames)
                .HasForeignKey(g => g.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Game>()
                .HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(g => g.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Bet>(r =>
            {
                r.HasMany(b => b.Games)
                .WithMany(g => g.Bets)
                .UsingEntity<BetItem>(
                    t => t.HasOne(bi => bi.Game)
                    .WithMany()
                    .HasForeignKey(bi => bi.GameId),

                    t => t.HasOne(bi => bi.Bet)
                    .WithMany()
                    .HasForeignKey(bi => bi.BetId),

                    ri =>
                    {
                        ri.HasKey(ri => ri.Id);
                    });
            });

            base.OnModelCreating(builder);
        }
    }
}
