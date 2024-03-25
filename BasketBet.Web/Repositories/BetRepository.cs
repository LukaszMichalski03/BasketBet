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
            this._context = context;
            this._mapper = mapper;
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
            await _context.SaveChangesAsync(); // Po tej operacji obiekt bet powinien mieć już przypisane ID.

            foreach (var singleGameBet in betVM.BetsList)
            {
                BetItem item = new BetItem()
                {
                    GameId = singleGameBet.GameVMId,
                    SelectedTeamId = singleGameBet.TeamTypedOnId,
                    BetId = bet.Id, // Tutaj bet.Id już istnieje po poprzednim SaveChanges()
                    BetItemOutcome = null,
                    ItemOdds = singleGameBet.Course
                };
                _context.BetItems.Add(item);
            }
            currentUser.Points -= bet.Bid;
            await _context.SaveChangesAsync(); // Zapisz zmiany dla BetItems

            return bet.Id; // Zwróć ID nowo utworzonego zakładu
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

    }
}
