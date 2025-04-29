using KejaHUnt_PropertiesAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace KejaHUnt_PropertiesAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<Unit> Units { get; set; }
    }
}
