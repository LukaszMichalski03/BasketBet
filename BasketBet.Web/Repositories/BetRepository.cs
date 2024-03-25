using AutoMapper;
using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
using BasketBet.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BasketBet.Web.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BetRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> CreateBet(BetVM betVM, AppUser currentUser)
        {
            Bet bet = new Bet()
            {
                AppUserId = currentUser.Id,
                TotalOdds = betVM.TotalCourse,
                Bid = betVM.Points,
                PotentialWinning = betVM.TotalWinning,
                BetOutcome = null,
            };
            _context.Bets.Add(bet);
            await _context.SaveChangesAsync();

            foreach (var singleGameBet in betVM.BetsList)
            {
                BetItem item = new BetItem()
                {
                    GameId = singleGameBet.GameVMId,
                    SelectedTeamId = singleGameBet.TeamTypedOnId,
                    BetId = bet.Id,
                    BetItemOutcome = null,
                    ItemOdds = singleGameBet.Course
                };
                _context.BetItems.Add(item);
            }
            currentUser.Points -= bet.Bid;
            await _context.SaveChangesAsync();

            return bet.Id; 
        }
        public async Task<BetVM> GetById(int id)
        {
            var betItems = await _context.BetItems.Include(bi => bi.Game).ThenInclude(g => g.HomeTeam).Include(bi => bi.Game).ThenInclude(g => g.AwayTeam)
                .Where(b => b.BetId == id).ToListAsync();
            var bet = await _context.Bets.Where(b => b.Id == id).FirstOrDefaultAsync();

            List<SingleGameBetVM> betsList = _mapper.Map<List<SingleGameBetVM>>(betItems);
            BetVM betVM = new()
            {
                BetsList = betsList,
                Points = bet.Bid,
                TotalCourse = bet.TotalOdds,
                TotalWinning = bet.PotentialWinning,
                BetOutcome = bet.BetOutcome
            };
            return betVM;
        }
        public async Task<List<BetVM>> GetUsersBets(string userId)
        {
            List<BetVM> betVMs = new List<BetVM>();
            var bets = await _context.Bets.Where(b => b.AppUserId == userId).OrderByDescending(b => b.Id).Take(20).ToListAsync();
            foreach(var bet in bets)
            {
                var betItems = _context.BetItems.Include(bi => bi.Game).ThenInclude(g => g.HomeTeam).Include(bi => bi.Game).ThenInclude(g => g.AwayTeam)
                .Where(b => b.BetId == bet.Id).ToList();
                List<SingleGameBetVM> betsList = _mapper.Map<List<SingleGameBetVM>>(betItems);
                betVMs.Add(new BetVM
                {
                    BetOutcome = bet.BetOutcome,
                    BetsList = betsList,
                    Points = bet.Bid,
                    TotalCourse = bet.TotalOdds,
                    TotalWinning = bet.PotentialWinning
                });
            }

            return betVMs;
        }
        public async Task CheckBetsOutcome()
        {
            List<Bet> betsInProcess = await _context.Bets.Include(b => b.Games).Where(b => b.BetOutcome == null).ToListAsync();
            List<BetItem> betItemsInProcess = await _context.BetItems
                .Include(bi => bi.Game)
                .Where(bi => bi.Game.HomeTeamScore != null && bi.Game.AwayTeamScore != null)
                .ToListAsync();

            betItemsInProcess = betItemsInProcess
                .Where(bi => betsInProcess.Any(b => b.Id == bi.BetId))
                .ToList();

            foreach (var betItem in betItemsInProcess)
            {
                if(betItem.SelectedTeamId == betItem.Game.AwayTeamId
                    && betItem.Game.AwayTeamScore > betItem.Game.HomeTeamScore)
                {
                    betItem.BetItemOutcome = true;
                }
                else if (betItem.SelectedTeamId == betItem.Game.HomeTeamId
                    && betItem.Game.AwayTeamScore < betItem.Game.HomeTeamScore)
                {
                    betItem.BetItemOutcome = true;
                }
                else betItem.BetItemOutcome = false;
            }
            await _context.SaveChangesAsync();
            foreach (var bet in betsInProcess)
            {
                var betItems = await _context.BetItems.Where(bi => bi.BetId == bet.Id).ToListAsync();

                bool allOutcomesTrue = betItems.All(bi => bi.BetItemOutcome == true);
                bool anyOutcomeFalse = betItems.Any(bi => bi.BetItemOutcome == false);

                if (allOutcomesTrue)
                {
                    if (bet.BetOutcome != true)
                    {
                        bet.BetOutcome = true;
                        var user = _context.Users.Where(u => u.Id == bet.AppUserId).FirstOrDefault();
                        user.Points += bet.PotentialWinning;
                    }
                    
                }
                else if (anyOutcomeFalse)
                {
                    bet.BetOutcome = false;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
