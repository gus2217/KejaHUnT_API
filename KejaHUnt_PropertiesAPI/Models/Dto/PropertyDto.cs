using KejaHUnt_PropertiesAPI.Models.Domain;

namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class PropertyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public List<UnitDto> Units { get; set; } = new List<UnitDto>();
    }
}
