using BasketBet.Web.ViewModels;

namespace BasketBet.Web.Interfaces
{
    public interface ITeamsRepository
    {
        Task<StandingsVM> GetTables();
    }
}
