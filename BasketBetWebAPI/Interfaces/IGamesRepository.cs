using BasketBetWebAPI.Models;

namespace BasketBetWebAPI.Interfaces
{
    public interface IGamesRepository
    {
        Task UpdateGames(List<GameDto> gamesDtos);
    }
}
