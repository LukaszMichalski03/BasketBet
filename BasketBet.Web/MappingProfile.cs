﻿using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamVM>();

            CreateMap<Game, GameVM>()
                .ForMember(dest => dest.HomeTeamVMId, opt => opt.MapFrom(src => src.HomeTeamId))
                .ForMember(dest => dest.AwayTeamVMId, opt => opt.MapFrom(src => src.AwayTeamId))
                .ForMember(dest => dest.HomeTeamVM, opt => opt.MapFrom(src => MapTeamVM(src.HomeTeam)))
                .ForMember(dest => dest.AwayTeamVM, opt => opt.MapFrom(src => MapTeamVM(src.AwayTeam)))
                .ForMember(dest => dest.OddsHomeTeam, opt => opt.MapFrom(src => Math.Round(src.OddsHomeTeam, 2)))
                .ForMember(dest => dest.OddsAwayTeam, opt => opt.MapFrom(src => Math.Round(src.OddsAwayTeam, 2)));

            CreateMap<SendBetVM, Bet>()
            .ForMember(dest => dest.TotalOdds, opt => opt.MapFrom(src => src.TotalCourse))
            .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Points))
            .ForMember(dest => dest.PotentialWinning, opt => opt.MapFrom(src => src.TotalWinning))
            .ForMember(dest => dest.Games, opt => opt.MapFrom(src => src.BetsList.Select(bet => new Game
            {
                Date = bet.GameVM.Date,
                HomeTeamId = bet.GameVM.HomeTeamVM.Id,
                HomeTeamScore = null, // Ustawiamy na null, bo wartość nie jest jeszcze znana
                OddsHomeTeam = bet.GameVM.OddsHomeTeam,
                AwayTeamId = bet.GameVM.AwayTeamVM.Id,
                AwayTeamScore = null, // Ustawiamy na null, bo wartość nie jest jeszcze znana
                OddsAwayTeam = bet.GameVM.OddsAwayTeam,
                Bets = new List<Bet> // Tworzymy pustą listę zakładów dla każdego meczu
                {
                    new Bet // Dodajemy nowy zakład do listy
                    {
                        TotalOdds = bet.Course,
                        Bid = src.Points,
                        PotentialWinning = src.TotalWinning
                    }
                }
            })));
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
