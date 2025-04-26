using KejaHUnT_TenantAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace KejaHUnT_TenantAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Unit> Units { get; set; }
    }
}
