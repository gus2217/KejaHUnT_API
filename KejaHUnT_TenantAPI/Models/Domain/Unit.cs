using Microsoft.EntityFrameworkCore;

namespace KejaHUnT_TenantAPI.Models.Domain
{
    public class Unit
    {
        public long Id { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int Bathrooms { get; set; }
        public double Size { get; set; }
        public int NoOfUnits { get; set; }
        public int PropertyId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
