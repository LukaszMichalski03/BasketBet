using AutoMapper;
using BasketBet.EntityFramework.Data;
using BasketBet.Web.Interfaces;
using BasketBet.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BasketBet.Web.Repositories
{
    public class TeamsRepository : ITeamsRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public TeamsRepository(DataContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<StandingsVM> GetTables()
        {
            var EasternConf = await _context.Teams.Where(t => t.Conference == "Eastern").OrderByDescending(t => t.WinningPercentage).ToListAsync();
            var WesternConf = await _context.Teams.Where(t => t.Conference == "Western").OrderByDescending(t => t.WinningPercentage).ToListAsync();
            var EasternVM = _mapper.Map<List<TeamVM>>(EasternConf);
            var WesternVM = _mapper.Map<List<TeamVM>>(WesternConf);

            return new StandingsVM
            {
                WesternConference = WesternVM,
                EasternConference = EasternVM,
            };

        }
    }
}
