using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
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
    }
}
