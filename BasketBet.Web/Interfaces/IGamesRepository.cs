using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface IGamesRepository
    {
        Task<List<GameVM>> GetLatestScores();
        Task<List<GameVM>> GetRecentGames();
    }
}
