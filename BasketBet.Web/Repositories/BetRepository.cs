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

        public async Task<int> CreateBet(SendBetVM betVM, AppUser currentUser)
        {
            Bet bet = new Bet()
            {
                AppUserId = currentUser.Id,
                TotalOdds = betVM.TotalCourse,
                Bid = betVM.Points,
                PotentialWinning = betVM.TotalWinning
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
                    BetItemOutcome = false // Musisz zdecydować, co robić w przypadku BetItemOutcome, który nie może być null w bool
                };
                _context.BetItems.Add(item);
            }

            await _context.SaveChangesAsync(); // Zapisz zmiany dla BetItems

            return bet.Id; // Zwróć ID nowo utworzonego zakładu
        }
        public async Task<Bet> GetById(int id)
        {
            //_context.Bets.Where(b => b.Id == id).FirstOrDefault();
            return new Bet();
        }
        
    }
}
