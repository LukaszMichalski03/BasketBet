using BasketBet.EntityFramework.Entities;
using BasketBet.Web.Interfaces;
using BasketBet.Web.Models;
using BasketBet.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net;
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
            _signIngManager = signIngManager;
            _userManager = userManager;
            _logger = logger;
            _gamesRepository = gamesRepository;
            _betRepository = betRepository;
            _userRepository = userRepository;
            _teamsRepository = teamsRepository;
        }
        [HttpPost]
        public async Task<IActionResult> CreateBet([FromBody] BetVM betVM)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Json(new { success = false, result = false });
            }
            int resultId = await _betRepository.CreateBet(betVM, currentUser);

            return Json(new { success = true, result = resultId });

        }
        [HttpGet]
        public async Task<IActionResult> NewBet(int BetId)
        {
            BetVM bet = await _betRepository.GetById(BetId);
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

            Task.Run(async () =>
            {
                using (_client)
                {
                    HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "Schedule", null);
                }
            });

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
            DateTime yesterday = DateTime.Today.AddDays(-1);
            string formattedDate = yesterday.ToString("yyyy-MM-dd");
            
            Task.Run(async () =>
            {
                using (_client)
                {
                    
                     HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + $"Schedule/Scores/{formattedDate}", null);
                }
            });
            await _betRepository.CheckBetsOutcome();
            var MyBets = await _betRepository.GetUsersBets(currentUser.Id);
            return View(MyBets);
        }
        public async Task<IActionResult> Standings()
        {
            Task.Run(async () =>
            {
                using (_client)
                {
                    HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "Table", null);
                }
            });
            var Standings = await _teamsRepository.GetTables();
            return View(Standings);
        }
        public async Task<IActionResult> LatestResults()
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            string formattedDate = yesterday.ToString("yyyyMMdd");

            Task.Run(async () =>
            {
                using (_client)
                {
                    HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + $"Schedule/Scores/{formattedDate}", null);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await _betRepository.CheckBetsOutcome();
                    }
                }
            });
            var games = await _gamesRepository.GetLatestScores();
            return View(games);
        }
        public async Task<IActionResult> Leaderboards()
        {
            LeaderBoardsVM standingsVM = new LeaderBoardsVM();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                standingsVM = await _userRepository.GetLeaderboards();
            }
            else standingsVM = await _userRepository.GetLeaderboards(currentUser);

            return View(standingsVM);
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
