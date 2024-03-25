

namespace BasketBet.EntityFramework.Entities
{
    public class BetItem
    {
        public int Id { get; set; }

        public int SelectedTeamId { get; set; }

        public double ItemOdds { get; set; }

        public int BetId { get; set; }
        public Bet Bet { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public bool? BetItemOutcome { get; set; }
    }
}
