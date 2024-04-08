using AutoMapper;
using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
using BasketBet.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BasketBet.Web.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GamesRepository(DataContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<List<GameVM>> GetRecentGames()
        {
            DateTime today = DateTime.Today;
            DateOnly todayDateOnly = new DateOnly(today.Year, today.Month, today.Day);

            List<Game> games = await _context.Games.Include(g => g.AwayTeam).Include(g => g.HomeTeam)
                .Where(g => g.Date >= todayDateOnly && g.HomeTeamScore == null && g.AwayTeamScore == null)
                .Take(20)
                .ToListAsync();

            List<GameVM> gameVMs = _mapper.Map<List<GameVM>>(games);

            return gameVMs;
        }
        public async Task<List<GameVM>> GetLatestScores()
        {
            List<Game> games = await _context.Games.Include(g => g.AwayTeam).Include(g => g.HomeTeam)
                .Where(g => g.AwayTeamScore != null && g.HomeTeamScore != null).OrderByDescending(g => g.Id).Take(40)
                .ToListAsync();
            List<GameVM> gameVMs = _mapper.Map<List<GameVM>>(games);
            return gameVMs;
        }

    }
}
