using Microsoft.EntityFrameworkCore;

namespace KejaHUnt_PropertiesAPI.Models.Domain
{
    public class Unit
    {
        public int Id { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int Bathrooms { get; set; }
        public double Size { get; set; }
        public int NoOfUnits { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; }

    }
}
