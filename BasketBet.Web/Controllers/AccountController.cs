using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BasketBet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly HttpClient _client;
        Uri baseAddress = new Uri("https://localhost:7041");

        public AccountController(SignInManager<AppUser> signIngManager, UserManager<AppUser> userManager)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            _signInManager = signIngManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user == null) user = await _userManager.FindByNameAsync(loginVM.Email);
                
                if(user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);

                    if (result.Succeeded)
                    {
                        await Task.WhenAll(
                        SendApiRequest("Schedule"),
                        SendApiRequest($"Schedule/{DateTime.Today.AddDays(-7):yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today:yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today.AddDays(-1):yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today.AddDays(-2):yyyy-MM-dd}")
                    );
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Invalid Password");
                    return View(loginVM);
                }
                ModelState.AddModelError("", "Account not found");

                
            }

            return View(loginVM);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken");
                    return View(registerVM);
                }
                AppUser user = new AppUser
                {
                    Points = 300,
                    UserName = registerVM.Username,
                    Email = registerVM.Email,
                    
                };
                var result = await _userManager.CreateAsync(user, registerVM.Password!);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    // Wykonaj zapytania do API w tle
                    await Task.WhenAll(
                        SendApiRequest("Schedule"),
                        SendApiRequest($"Schedule/{DateTime.Today.AddDays(-7):yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today:yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today.AddDays(-1):yyyy-MM-dd}"),
                        SendApiRequest($"Schedule/Scores/{DateTime.Today.AddDays(-2):yyyy-MM-dd}")
                    );
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(registerVM);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
        private async Task SendApiRequest(string endpoint)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.PutAsync(_client.BaseAddress + endpoint, null))
                {
                    
                }
            }
        }
    }
}
