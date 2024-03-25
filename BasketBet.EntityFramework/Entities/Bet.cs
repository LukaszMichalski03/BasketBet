using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketBet.EntityFramework.Entities
{
    public class Bet
    {
        public int Id { get; set; }

        public double TotalOdds { get; set; }
        public double Bid { get; set; }
        public double PotentialWinning { get; set; }

        public bool? BetOutcome { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();      
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        
    }
}
