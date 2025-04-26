using KejaHUnT_TenantAPI.Models.Domain;

namespace KejaHUnT_TenantAPI.Models.Dto
{
    public class TenantDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int IdNo { get; set; }
        public string Email { get; set; }
        public string Employer { get; set; }
        public List<UnitDto> Units { get; set; } = new List<UnitDto>();
        public int PropertyId { get; set; }
    }
}
