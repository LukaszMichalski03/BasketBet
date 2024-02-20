using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Models;
using BasketBetWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BasketBetWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableController : ControllerBase
    {
        private readonly Scrapper _scrapper;
        private readonly ITeamsRepository _teamsRepository;

        public TableController(Scrapper scrapper, ITeamsRepository teamsRepository)
        {
            _scrapper = scrapper;
            _teamsRepository = teamsRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> Get()
        {
            var teamsDtos = await _teamsRepository.GetAll();
            return Ok(teamsDtos);
        }
        [HttpPut]
        public async Task<IActionResult> Refresh()
        {
            await _scrapper.UpdateTable();
            return NoContent();
        }
    }
}
