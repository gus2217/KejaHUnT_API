using KejaHUnt_PropertiesAPI.Models.Domain;

namespace KejaHUnt_PropertiesAPI.Repositories.Interface
{
    public interface IUnitRepository
    {
        Task<Unit> CreateUnitAsync(Unit unit);

        Task<IEnumerable<Unit>> GetAllAsync();

        Task<Unit?> GetUnitByIdAsync(long id);

        Task<Unit?> UpdateAsync(Unit unit);


        Task<Unit?> DeleteAync(long id);
        Task<IEnumerable<Unit>> GetUnitsByPropertyIdAsync(long propertyId);

    }
}
