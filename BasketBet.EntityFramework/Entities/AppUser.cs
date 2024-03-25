using Microsoft.AspNetCore.Identity;

namespace BasketBet.EntityFramework.Entities
{
    public class AppUser : IdentityUser
    {       
        public double Points { get; set; }
        public DateTime LastPointsClaimTime { get; set; }
        public ICollection<Bet> Bets { get; set; }
    }
}
