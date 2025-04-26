namespace KejaHUnT_TenantAPI.Models.Dto
{
    public class UpdateTenantRequestDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int IdNo { get; set; }
        public string Email { get; set; }
        public string Employer { get; set; }
        public List<UpdateUnitRequestDto> Units { get; set; } = new List<UpdateUnitRequestDto>();
    }
}
