namespace KejaHUnT_TenantAPI.Models.Domain
{
    public class Tenant : Base
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int IdNo { get; set; }
        public string Email { get; set; }
        public string Employer { get; set; }
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
        public int PropertyId { get; set; }
    }
}
