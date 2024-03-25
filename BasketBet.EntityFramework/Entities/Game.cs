

namespace BasketBet.EntityFramework.Entities
{
    public class Game
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }

        public int? HomeTeamId { get; set; }

        public virtual Team HomeTeam { get; set; }
        public int? HomeTeamScore { get; set; }
        public double OddsHomeTeam { get; set; }

        public int? AwayTeamId { get; set; }

        public virtual Team AwayTeam { get; set; }
        public int? AwayTeamScore { get; set; }
        public double OddsAwayTeam { get; set; }

        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
