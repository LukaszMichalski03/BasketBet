using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBetWebAPI.Models;

namespace BasketBetWebAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();

            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>()
                .ForMember(g => g.AwayTeamId, x => x.MapFrom(dto => dto.AwayTeamDtoId))
                .ForMember(g => g.HomeTeamId, x => x.MapFrom(dto => dto.HomeTeamDtoId));

        }
    }
}
