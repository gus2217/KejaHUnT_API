using KejaHUnt_PropertiesAPI.Models.Domain;

namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class CreatePropertyRequestDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public List<CreateUnitRequestDto> Units { get; set; } = new List<CreateUnitRequestDto>();
    }
}
