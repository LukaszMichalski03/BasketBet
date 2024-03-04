using System.ComponentModel.DataAnnotations;

namespace BasketBet.Web.ViewModels
{
    public class LoginVM
    {        
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }      
        public bool RememberMe { get; set; }
    }
}
