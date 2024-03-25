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

        public ScheduleController(Scrapper scrapper)
        {
            _scrapper = scrapper;
        }
        [HttpPut("{date}")]
        public async Task<IActionResult> UpdateGamesFromDate([FromRoute] DateOnly date)
        {
            await _scrapper.UpdateGamesFromDate(date);
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> Refresh()
        {
            await _scrapper.UpdateGames();
            return NoContent();
        }
        

        [HttpPut("scores/{date}")]
        public async Task<IActionResult> UpdateScores([FromRoute] DateOnly date)
        {
            bool result = await _scrapper.UpdateGamesResults(date);
            if (result) return Ok();
            return NoContent();
        }

    }
}
