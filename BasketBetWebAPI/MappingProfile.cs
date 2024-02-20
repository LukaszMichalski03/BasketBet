using AutoMapper;
using BasketBetWebAPI.Entities;
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
            CreateMap<GameDto, Game>();
        }
    }
}
