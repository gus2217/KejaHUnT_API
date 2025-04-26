namespace KejaHUnT_TenantAPI.Models.Dto
{
    public class CreateTenantRequestDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int IdNo { get; set; }
        public string Email { get; set; }
        public string Employer { get; set; }
        public List<CreateUnitRequestDto> Units { get; set; } = new List<CreateUnitRequestDto>();
        public int PropertyId { get; set; }
        public string CreatedBy { get; set; }
    }
}
