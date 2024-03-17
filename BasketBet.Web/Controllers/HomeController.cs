using BasketBet.Web.Interfaces;
using BasketBet.Web.Models;
using BasketBet.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;

namespace BasketBet.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGamesRepository _gamesRepository;
        private readonly HttpClient _client;
        Uri baseAddress = new Uri("https://localhost:7041");

        public HomeController(ILogger<HomeController> logger, IGamesRepository gamesRepository)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
            _logger = logger;
            this._gamesRepository = gamesRepository;
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
