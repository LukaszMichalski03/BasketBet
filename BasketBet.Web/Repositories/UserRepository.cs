using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
using BasketBet.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BasketBet.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public async Task ClaimPoints(string userId)
        {
            AppUser user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            user.Points += 100;
            user.LastPointsClaimTime = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        public async Task<LeaderBoardsVM> GetLeaderboards(AppUser? currentUser = null)
        {
            LeaderBoardsVM leaderboard = new();
            List<UserVM> leaders = await _context.Users
                .Include(u => u.Bets)
                .OrderByDescending(u => u.Points)
                .Take(50)
                .Select(u => new UserVM
                {
                    Name = u.UserName,
                    Points = u.Points,
                    BetSuccessRate = CalculateBetSuccessRate(u)
                })
                .ToListAsync();
            leaderboard.Leaders = leaders;
            if (currentUser != null)
            {
                var user = _context.Users.Include(u => u.Bets).Where(u => u.Id == currentUser.Id).FirstOrDefault();
                int? currentUserPosition = leaders.FindIndex(u => u.Name == currentUser.UserName);
                leaderboard.CurrentUserPosition = currentUserPosition != -1 ? currentUserPosition + 1 : (int?)null;
                leaderboard.CurrentUser = new UserVM
                {
                    Name = currentUser.UserName,
                    Points = currentUser.Points,
                    BetSuccessRate = CalculateBetSuccessRate(user),
                    
                };
            }
            return leaderboard;
        }
        public static double CalculateBetSuccessRate(AppUser user)
        {
           
            if (user.Bets == null || !user.Bets.Any())
            {
                return 0.0;
            }

            int totalBets = user.Bets.Count;
            int successfulBets = user.Bets.Count(b => b.BetOutcome == true);

            double successRate = (double)successfulBets / totalBets * 100;
            return Math.Round(successRate, 2);
        }

    }
}
