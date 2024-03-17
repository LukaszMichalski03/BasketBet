using AutoMapper;
using BasketBet.EntityFramework.Data;
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

            List<GameVM> games = await _context.Games.Include(g => g.AwayTeam).Include(g => g.HomeTeam)
                .Where(g => g.Date == todayDateOnly)
                .Select(g => new GameVM
                {
                    HomeTeamVM = _mapper.Map<TeamVM>(g.HomeTeam),
                    AwayTeamVM = _mapper.Map<TeamVM>(g.AwayTeam),

                })
                .ToListAsync();

            return games;
        }
    }
}
