using AutoMapper;
using BasketBet.EntityFramework.Data;
using BasketBet.EntityFramework.Entities;
using BasketBetWebAPI.Exceptions;
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

            DateOnly dateToCheckFirst = games.First().Date;
            DateOnly dateToCheckLast = games.Last().Date;
            List<Game> existingGames = await _context.Games
                .Where(g => g.Date >= dateToCheckFirst && g.Date <= dateToCheckLast)
                .ToListAsync();


            foreach (var game in games)
            {
                bool gameExistsInMemory = existingGames.Any(g => g.HomeTeamId == game.HomeTeamId && g.AwayTeamId == game.AwayTeamId && g.Date == game.Date);

                if (!gameExistsInMemory)
                {
                    _context.Games.Add(game);
                }
            }
            await _context.SaveChangesAsync();
        }
        public async Task<List<GameDto>> GetGamesByDate(DateOnly date)
        {
            List<Game> games = await _context.Games.Where(g => g.Date == date).ToListAsync();
            return _mapper.Map<List<GameDto>>(games);
        } 
        public async Task<int> UpdateGamesScores(List<GameDto> gameDtos)
        {
            List<Game> games = await _context.Games.Include(g => g.AwayTeam).Include(g => g.HomeTeam).Where(g => g.Date == gameDtos[0].Date).ToListAsync();
            foreach(var gameDto in  gameDtos)
            {
                var hometeam = await _context.Teams.FirstOrDefaultAsync(t => t.Name.Contains(gameDto.HomeTeamDto.Name));
                var awayteam = await _context.Teams.FirstOrDefaultAsync(t => t.Name.Contains(gameDto.AwayTeamDto.Name));
                if (hometeam == null || awayteam == null) throw new NotFoundException("Team not found");
                Game? gameExists = games.FirstOrDefault(g => g.Date == gameDto.Date && g.HomeTeam.Name == hometeam.Name && g.AwayTeam.Name == awayteam.Name);

                if (gameExists != null)
                {
                    gameExists.HomeTeamScore = gameDto.HomeTeamScore;
                    gameExists.AwayTeamScore = gameDto.AwayTeamScore;
                }
                else _context.Add(new Game
                {
                    Date = gameDto.Date,
                    HomeTeam = hometeam,
                    HomeTeamId = hometeam.Id,
                    HomeTeamScore = gameDto.HomeTeamScore,
                    AwayTeam = awayteam,
                    AwayTeamId = awayteam.Id,
                    AwayTeamScore = gameDto.AwayTeamScore,
                    

                });
            }
            int result = await _context.SaveChangesAsync();
            return result;
        }

    }
}
