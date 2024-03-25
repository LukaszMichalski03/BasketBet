using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BetItem, SingleGameBetVM>()
                .ForMember(s => s.Course, opt => opt.MapFrom(b => b.ItemOdds))
                .ForMember(s => s.TeamTypedOn, opt => opt.MapFrom(b =>
                    b.SelectedTeamId == b.Game.AwayTeamId ? MapTeamVM(b.Game.AwayTeam) : MapTeamVM(b.Game.HomeTeam)))
                .ForMember(s => s.GameVM, opt => opt.MapFrom(b => MapGameVM(b.Game)))
                .ForMember(s => s.BetItemOutcome, opt => opt.MapFrom(b => b.BetItemOutcome));



            CreateMap<Team, TeamVM>();

            CreateMap<Game, GameVM>()
                .ForMember(dest => dest.HomeTeamVMId, opt => opt.MapFrom(src => src.HomeTeamId))
                .ForMember(dest => dest.AwayTeamVMId, opt => opt.MapFrom(src => src.AwayTeamId))
                .ForMember(dest => dest.HomeTeamScore, opt => opt.MapFrom(src => src.HomeTeamScore))
                .ForMember(dest => dest.AwayTeamScore, opt => opt.MapFrom(src => src.AwayTeamScore))
                .ForMember(dest => dest.HomeTeamVM, opt => opt.MapFrom(src => MapTeamVM(src.HomeTeam)))
                .ForMember(dest => dest.AwayTeamVM, opt => opt.MapFrom(src => MapTeamVM(src.AwayTeam)))
                .ForMember(dest => dest.OddsHomeTeam, opt => opt.MapFrom(src => Math.Round(src.OddsHomeTeam, 2)))
                .ForMember(dest => dest.OddsAwayTeam, opt => opt.MapFrom(src => Math.Round(src.OddsAwayTeam, 2)));

            CreateMap<BetVM, Bet>()
            .ForMember(dest => dest.TotalOdds, opt => opt.MapFrom(src => src.TotalCourse))
            .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Points))
            .ForMember(dest => dest.BetOutcome, opt => opt.MapFrom(src => src.BetOutcome))
            .ForMember(dest => dest.PotentialWinning, opt => opt.MapFrom(src => src.TotalWinning))
            .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.BetsList.Select(bet => new Game
            {
                Date = bet.GameVM.Date,
                HomeTeamId = bet.GameVM.HomeTeamVM.Id,
                HomeTeamScore = null,
                OddsHomeTeam = bet.GameVM.OddsHomeTeam,
                AwayTeamId = bet.GameVM.AwayTeamVM.Id,
                AwayTeamScore = null,
                OddsAwayTeam = bet.GameVM.OddsAwayTeam,
                Bets = new List<Bet>
                {
                    new Bet
                    {
                        TotalOdds = bet.Course,
                        Bid = src.Points,
                        PotentialWinning = src.TotalWinning
                    }
                }
            })));
        }
        private GameVM MapGameVM(Game game)
        {
            if (game == null)
                return null;

            return new GameVM
            {
                Id = game.Id,
                Date = game.Date,
                HomeTeamVMId = game.HomeTeamId,
                AwayTeamVMId = game.AwayTeamId,
                HomeTeamVM = MapTeamVM(game.HomeTeam),
                AwayTeamVM = MapTeamVM(game.AwayTeam),
                OddsHomeTeam = Math.Round(game.OddsHomeTeam, 2),
                OddsAwayTeam = Math.Round(game.OddsAwayTeam, 2)
            };
        }

        private TeamVM MapTeamVM(Team team)
        {
            if (team == null)
                return null;

            return new TeamVM
            {
                Id = team.Id,
                Name = team.Name,
                Wins = team.Wins,
                Looses = team.Looses,
                WinningPercentage = team.WinningPercentage,
                HomeRecord = team.HomeRecord,
                AwayRecord = team.AwayRecord,
                PointsPerGame = team.PointsPerGame,
                OpponentPointsPerGame = team.OpponentPointsPerGame,
                CurrentStreak = team.CurrentStreak,
                Last10Record = team.Last10Record,
                Conference = team.Conference
            };
        }
    }
}
