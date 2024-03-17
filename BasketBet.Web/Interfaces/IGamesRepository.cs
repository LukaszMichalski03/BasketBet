using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface IGamesRepository
    {
        Task<List<GameVM>> GetRecentGames();
    }
}
