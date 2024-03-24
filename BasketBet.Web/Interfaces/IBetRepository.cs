using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface IBetRepository
    {
        Task<int> CreateBet(BetVM betVM, AppUser currentUser);
        Task<BetVM> GetById(int id);
    }
}
