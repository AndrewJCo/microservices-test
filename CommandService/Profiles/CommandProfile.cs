using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandProfile : Profile
    {
        public CommandProfile()
        {
            // Source to target
             CreateMap<Platform, PlatformReadDto>();
             CreateMap<CommandCreateDto, Command>();
             CreateMap<Command, CommandReadDto>();
             CreateMap<PlatformPublishedDto, Platform>().ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
             CreateMap<GrpcPlatformModel, Platform>().ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId));
        }
    }
}