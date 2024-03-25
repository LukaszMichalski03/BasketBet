namespace BasketBetWebAPI.Models
{
    public class GameDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int HomeTeamDtoId { get; set; }
        public TeamDto HomeTeamDto { get; set; }
        public int? HomeTeamScore { get; set; }
        public int AwayTeamDtoId { get; set; }
        public TeamDto AwayTeamDto { get; set; }
        public int? AwayTeamScore { get; set; }
        public double OddsHomeTeam { get; set; }
        public double OddsAwayTeam { get; set; }
    }
}
