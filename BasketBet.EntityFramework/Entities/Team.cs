using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketBet.EntityFramework.Entities
{
    public class Team
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

        public virtual ICollection<Game> HomeGames { get; set; }
        public virtual ICollection<Game> AwayGames { get; set; }
    }
}
