using AutoMapper;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using Newtonsoft.Json;
using static Azure.Core.HttpHeader;

namespace KejaHUnt_PropertiesAPI.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Property -> CreatePropertyRequestDto
            CreateMap<Property, CreatePropertyRequestDto>()
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Units)));

            // CreatePropertyRequestDto -> Property
            CreateMap<CreatePropertyRequestDto, Property>()
                .ForMember(dest => dest.Units, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Units)
                        ? new List<Unit>()
                        : JsonConvert.DeserializeObject<List<Unit>>(src.Units)));

            CreateMap<Unit, CreateUnitRequestDto>().ReverseMap();

            CreateMap<Unit, UnitDto>().ReverseMap();
            
            CreateMap<UpdateUnitRequestDto, Unit>().ReverseMap();

            CreateMap<Property, PropertyDto>().ReverseMap();
        }
    }
}
