using AutoMapper;
using HomeAutomation.BlazorApp.Shared;
using HomeAutomation.Core.Model;

namespace HomeAutomation.BlazorApp.Server.Infrastructure
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Device, Light>();
        }
    }
}
