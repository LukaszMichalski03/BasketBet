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
        private readonly IUserRepository _userRepository;
        private readonly ITeamsRepository _teamsRepository;
        private readonly HttpClient _client;
        Uri baseAddress = new Uri("https://localhost:7041");

        public HomeController(SignInManager<AppUser> signIngManager, UserManager<AppUser> userManager, ILogger<HomeController> logger,
            IGamesRepository gamesRepository, IBetRepository betRepository, IUserRepository userRepository, ITeamsRepository teamsRepository)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            this._signIngManager = signIngManager;
            this._userManager = userManager;
            _logger = logger;
            this._gamesRepository = gamesRepository;
            this._betRepository = betRepository;
            this._userRepository = userRepository;
            this._teamsRepository = teamsRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateBet([FromBody] BetVM betVM)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                // U¿ytkownik niezalogowany, zwróæ odpowiedŸ z b³êdem
                return RedirectToAction("Login", "Account");
            }
            int resultId = await _betRepository.CreateBet(betVM, currentUser);

            return Json(new { success = true, result = resultId });

        }
        [HttpGet]
        public async Task<IActionResult> NewBet(int BetId)
        {
            BetVM bet = await _betRepository.GetById(BetId);
            // Pe³na œcie¿ka do widoku
            return View("~/Views/Home/NewBet.cshtml", bet);
        }
        [HttpPost]
        public async Task<IActionResult> ClaimPoints()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            await _userRepository.ClaimPoints(currentUser.Id);
            return Ok();
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            HomeVM vm = new HomeVM();
            vm.Matches = await _gamesRepository.GetRecentGames();
            if (currentUser != null) vm.LastPointsClaimTime = currentUser.LastPointsClaimTime;
            else vm.LastPointsClaimTime = null;
            return View(vm);
        }
        public async Task<IActionResult> MyBets()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var MyBets = await _betRepository.GetUsersBets(currentUser.Id);
            return View(MyBets);
        }
        public async Task<IActionResult> Standings()
        {
            var Standings = await _teamsRepository.GetTables();
            return View(Standings);
        }
        public async Task<IActionResult> LatestResults()
        {
            var games = await _gamesRepository.GetLatestScores();
            return View(games);
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
