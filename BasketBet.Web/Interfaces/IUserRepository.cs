
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface IUserRepository
    {
        Task ClaimPoints(string userId);
        Task<LeaderBoardsVM> GetLeaderboards(AppUser? currentUser = null);
    }
}
