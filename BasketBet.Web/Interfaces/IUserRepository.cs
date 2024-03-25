
namespace BasketBet.Web.Interfaces
{
    public interface IUserRepository
    {
        Task ClaimPoints(string userId);
    }
}
