namespace BasketBet.Web.ViewModels
{
    public class SingleGameBetVM
    {
        public int Id { get; set; }
        public int GameVMId { get; set; }
        public GameVM GameVM { get; set; }
        public int TeamTypedOnId { get; set; }
        public TeamVM TeamTypedOn { get; set; }
        public double Course {  get; set; }
        public bool? BetItemOutcome { get; set; } = null;
    }
}
