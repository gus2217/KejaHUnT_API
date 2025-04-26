using KejaHUnt_PropertiesAPI.Models.Domain;

namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class UpdatePropertyRequestDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public List<UpdateUnitRequestDto> Units { get; set; } = new List<UpdateUnitRequestDto>();
    }
}
