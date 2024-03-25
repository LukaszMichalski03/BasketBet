using BasketBet.EntityFramework.Entities;
using System.Text.RegularExpressions;

namespace BasketBet.Web.ViewModels
{
    public class HomeVM
    {
        public List<GameVM> Matches { get; set; } = new List<GameVM>();
        public DateTime? LastPointsClaimTime { get; set; }
    }
}
