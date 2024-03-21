using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
using BasketBet.Web.Models;
using BasketBet.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;

namespace BasketBet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<AppUser> _signIngManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;
        private readonly IGamesRepository _gamesRepository;
        private readonly IBetRepository _betRepository;
        private readonly HttpClient _client;
        Uri baseAddress = new Uri("https://localhost:7041");

        public HomeController(SignInManager<AppUser> signIngManager, UserManager<AppUser> userManager, ILogger<HomeController> logger,
            IGamesRepository gamesRepository, IBetRepository betRepository)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            this._signIngManager = signIngManager;
            this._userManager = userManager;
            _logger = logger;
            this._gamesRepository = gamesRepository;
            this._betRepository = betRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateBet([FromBody]SendBetVM betVM)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // U¿ytkownik niezalogowany, zwróæ odpowiedŸ z b³êdem
                return RedirectToAction("Login", "Account");
            }
            var resultId = await _betRepository.CreateBet(betVM, currentUser);
            
            return RedirectToAction("NewBet", resultId);
            
        }
        [HttpGet]
        public async Task<IActionResult> NewBet(int BetId)
        {
            return View(); ///////////
        }
        public async Task<IActionResult> Index()
        {
            HomeVM vm = new HomeVM();
            vm.Matches = await _gamesRepository.GetRecentGames();
            return View(vm);
        }
        [HttpGet]
        public IActionResult Table()
        {
            List<TeamVM> teams = new List<TeamVM>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "Table").Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                teams = JsonConvert.DeserializeObject<List<TeamVM>>(data);
            }
            return View(teams);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
