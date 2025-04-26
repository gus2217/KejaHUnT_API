using AutoMapper;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using static Azure.Core.HttpHeader;

namespace KejaHUnt_PropertiesAPI.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Property, CreatePropertyRequestDto>().ReverseMap();
            CreateMap<Unit, CreateUnitRequestDto>().ReverseMap();
            CreateMap<Unit, UnitDto>().ReverseMap();
            CreateMap<Property, PropertyDto>().ReverseMap();
        }
    }
}
