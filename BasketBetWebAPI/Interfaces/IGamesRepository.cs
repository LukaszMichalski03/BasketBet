using BasketBetWebAPI.Models;

namespace BasketBetWebAPI.Interfaces
{
    public interface IGamesRepository
    {
        Task<List<GameDto>> GetGamesByDate(DateOnly date);
        Task UpdateGames(List<GameDto> gamesDtos);
        Task<int> UpdateGamesScores(List<GameDto> gameDtos);
    }
}
