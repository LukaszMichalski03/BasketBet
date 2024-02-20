using BasketBetWebAPI.Models;

namespace BasketBetWebAPI.Interfaces
{
    public interface ITeamsRepository
    {
        Task<List<TeamDto>> GetAll();
        Task<TeamDto> GetByName(string name);
        Task Update(List<TeamDto> teams);
    }
}
