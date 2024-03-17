namespace BasketBet.Web.ViewModels
{
    public class SingleGameBetVM
    {
        public int Id { get; set; }
        public GameVM GameVM { get; set; }
        public TeamVM TeamTypedOn { get; set; }
        public DateOnly Date { get; set; }
        public double Course {  get; set; }

    }
}
