using KejaHUnt_PropertiesAPI.Data;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace KejaHUnt_PropertiesAPI.Repositories.Implementation
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> CreateUnitAsync(Unit unit)
        {
            await _dbContext.Units.AddAsync(unit);
            await _dbContext.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit?> DeleteAync(long id)
        {
            var existingUnit = await _dbContext.Units.FirstOrDefaultAsync(x => x.Id == id);

            if (existingUnit != null)
            {
                _dbContext.Units.Remove(existingUnit);
                await _dbContext.SaveChangesAsync();
                return existingUnit;
            }
            return null;
        }

        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            return await _dbContext.Units.ToListAsync();
        }

        public async Task<Unit?> GetUnitByIdAsync(long id)
        {
            return await _dbContext.Units.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Unit?> UpdateAsync(Unit unit)
        {
            var existingUnit = await _dbContext.Units.FirstOrDefaultAsync(x => x.Id == unit.Id);

            if (existingUnit == null)
            {
                return null;
            }
            // Update basic fields
            existingUnit.Price = unit.Price;
            existingUnit.Type = unit.Type;
            existingUnit.Bathrooms = unit.Bathrooms;
            existingUnit.Size = unit.Size;
            existingUnit.NoOfUnits = unit.NoOfUnits;
            existingUnit.PropertyId = unit.PropertyId;
            existingUnit.DocumentId = unit.DocumentId;

            // Save changes
            await _dbContext.SaveChangesAsync();

            return existingUnit;
        }

        public async Task<IEnumerable<Unit>> GetUnitsByPropertyIdAsync(long propertyId)
        {
            return await _dbContext.Units
                .Where(u => u.PropertyId == propertyId)
                .ToListAsync();
        }

    }
}
