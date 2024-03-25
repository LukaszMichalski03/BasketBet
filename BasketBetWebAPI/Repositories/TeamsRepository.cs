using AutoMapper;
using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBetWebAPI.Exceptions;
using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBetWebAPI.Repositories
{
    public class TeamsRepository : ITeamsRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TeamsRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<TeamDto>> GetAll()
        {
            List<Team> teams = await _context.Teams.ToListAsync();
            if (teams == null) throw new NotFoundException("No teams in the database");
            List<TeamDto> teamsDto = _mapper.Map<List<TeamDto>>(teams);
            return teamsDto;
        }
        public async Task Update(List<TeamDto> teamsDtos)
        {
            var teams = _mapper.Map<List<Team>>(teamsDtos);
            foreach (var team in teams)
            {
                Team? item = _context.Teams.FirstOrDefault(t => t.Name == team.Name);
                if (item != null)
                {
                    item.Name = team.Name;
                    item.Wins = team.Wins;
                    item.Looses = team.Looses;
                    item.WinningPercentage = team.WinningPercentage;
                    item.HomeRecord = team.HomeRecord;
                    item.AwayRecord = team.AwayRecord;
                    item.PointsPerGame = team.PointsPerGame;
                    item.OpponentPointsPerGame = team.OpponentPointsPerGame;
                    item.CurrentStreak = team.CurrentStreak;
                    item.Last10Record = team.Last10Record;
                    item.Conference = team.Conference;
                }
                else _context.Teams.Add(team);

                await _context.SaveChangesAsync();
            }
        }
        public async Task<TeamDto> GetByName(string name)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Name.StartsWith(name));
            if (team == null) throw new NotFoundException("Team not found");
            return _mapper.Map<TeamDto>(team);
        }
        
    }
}
