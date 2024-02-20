using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BasketBetWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly Scrapper _scrapper;
        private readonly IGamesRepository _gamesRepository;

        public ScheduleController(Scrapper scrapper,IGamesRepository gamesRepository)
        {
            _scrapper = scrapper;
            _gamesRepository = gamesRepository;
        }

        [HttpPut]
        public async Task<IActionResult> Refresh()
        {
            await _scrapper.UpdateGames();
            return NoContent();
        }
        //[HttpPut]
        //public async Task<IActionResult> UpdateGamesByDate([FromBody] DateOnly date)
        //{

        //    return NoContent();
        //}

        [HttpPut("{date}")]
        public async Task<IActionResult> UpdateScores([FromRoute] DateOnly date)
        {
            await _scrapper.UpdateGamesResults(date);
            return NoContent();
        }

    }
}
