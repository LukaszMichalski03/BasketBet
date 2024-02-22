namespace BasketBet.Web.ViewModels
{
    public class GameVM
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int HomeTeamDtoId { get; set; }
        public TeamVM HomeTeamDto { get; set; }
        public int? HomeTeamScore { get; set; }
        public int AwayTeamDtoId { get; set; }
        public TeamVM AwayTeamDto { get; set; }
        public int? AwayTeamScore { get; set; }
        public double OddsHomeTeam { get; set; }
        public double OddsAwayTeam { get; set; }
    }
}
