namespace BasketBet.Web.ViewModels
{
    public class SendBetVM
    {        
        public List<SingleGameBetVM> BetsList { get; set; }
        public double TotalCourse { get; set; }
        public double Points { get; set; }
        public double TotalWinning { get; set; }
    }
}
