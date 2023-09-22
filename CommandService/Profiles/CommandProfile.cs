using CommandService.Models;
using CommandService.Dtos;
using AutoMapper;
using PlatformService;
namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishDto, Platform>().ForMember(p => p.ExternalId, opt => opt.MapFrom(p => p.Id));
            CreateMap<GrpcPlatformModel, Platform>().ForMember(d => d.ExternalId, opt => opt.MapFrom(d => d.PlatformId));
        }
    }
}