using AutoMapper;
using BasketBetWebAPI.Entities;
using BasketBetWebAPI.Interfaces;
using BasketBetWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBetWebAPI.Repositories
{
    public class GamesRepository : IGamesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public GamesRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task UpdateGames(List<GameDto> gamesDtos)
        {
            List<Game> games = _mapper.Map<List<Game>>(gamesDtos);

            DateOnly dateToCheck = games.First().Date;
            List<Game> existingGames = await _context.Games.Where(g => g.Date == dateToCheck).ToListAsync();

            foreach (var game in games)
            {
                bool gameExistsInMemory = existingGames.Any(g => g.HomeTeamId == game.HomeTeamId && g.AwayTeamId == game.AwayTeamId);

                if (!gameExistsInMemory)
                {
                    _context.Games.Add(game);
                }
            }
            await _context.SaveChangesAsync();
        }

    }
}
