using AutoMapper;
using BasketBet.EntityFramework.Entities;
using BasketBet.Web.ViewModels;

namespace BasketBet.Web
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TeamVM, Team>();
            CreateMap<Team, TeamVM>();

            CreateMap<Game, GameVM>();
            CreateMap<GameVM, Game>()
                .ForMember(g => g.AwayTeamId, x => x.MapFrom(dto => dto.AwayTeamVMId))
                .ForMember(g => g.HomeTeamId, x => x.MapFrom(dto => dto.HomeTeamVMId));

           
        }
    }
}
