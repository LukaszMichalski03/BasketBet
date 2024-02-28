
namespace BasketBetWebAPI.Models
{
    public class TeamDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int Wins { get; set; }
        public int Looses { get; set; }
        public double WinningPercentage { get; set; }
        public string HomeRecord { get; set; }
        public string AwayRecord { get; set; }

        public double PointsPerGame { get; set; }
        public double OpponentPointsPerGame { get; set; }

        public string CurrentStreak { get; set; }
        public string Last10Record { get; set; }
        public string Conference { get; set; }

        public ICollection<GameDto> HomeGames { get; set; } = new List<GameDto>();
        public ICollection<GameDto> AwayGames { get; set; } = new List<GameDto>();
    }
}
