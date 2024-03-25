namespace BasketBet.Web.ViewModels
{
    public class LeaderBoardsVM
    {

        public int? CurrentUserPosition { get; set; }
        public UserVM? CurrentUser { get; set; }
        public List<UserVM> Leaders { get; set; }
    }
}
