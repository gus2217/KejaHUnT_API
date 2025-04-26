using KejaHUnT_TenantAPI.Models.Domain;
using KejaHUnT_TenantAPI.Models.Dto;

namespace KejaHUnT_TenantAPI.Repositories.Interface
{
    public interface ITenantRepository
    {
        Task<Tenant> CreateTenantAsync(Tenant tenant);

        Task<IEnumerable<Tenant>> GetAllAsync();

        Task<Tenant?> GetTenantByIdAsync(long id);

        Task<Tenant?> UpdateAsync(long id, UpdateTenantRequestDto tenant);

        Task<Tenant?> DeleteAync(long id);
    }
}
