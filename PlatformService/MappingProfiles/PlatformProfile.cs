using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.MappingProfiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishDto>();
            CreateMap<Platform, GrpcPlatformModel>().ForMember(d => d.PlatformId, opt => opt.MapFrom(s => s.Id));
        }
    }
}