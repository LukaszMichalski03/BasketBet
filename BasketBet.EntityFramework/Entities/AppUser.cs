using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BasketBet.EntityFramework.Entities
{
    public class AppUser : IdentityUser
    {       
        public double Points { get; set; }
        public DateTime LastPointsClaimTime { get; set; }
        public ICollection<Bet> Bets { get; set; }
    }
}
