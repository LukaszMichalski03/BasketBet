namespace BasketBet.Web.ViewModels
{
    public class BetVM
    {        
        public List<SingleGameBetVM> BetsList { get; set; }
        public double TotalCourse { get; set; }
        public double Points { get; set; }
        public double TotalWinning { get; set; }
        public bool? BetOutcome { get; set; }
    }
}
