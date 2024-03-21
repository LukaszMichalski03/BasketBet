using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface IBetRepository
    {
        Task<int> CreateBet(SendBetVM betVM, AppUser currentUser);
    }
}
