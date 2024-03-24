namespace BasketBet.Web.ViewModels
{
    public class GameVM
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int? HomeTeamVMId { get; set; }
        public TeamVM HomeTeamVM { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamVMId { get; set; }
        public TeamVM AwayTeamVM { get; set; }
        public int? AwayTeamScore { get; set; }
        public double OddsHomeTeam { get; set; }
        public double OddsAwayTeam { get; set; }
    }
}
