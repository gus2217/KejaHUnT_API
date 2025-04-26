using KejaHUnt_PropertiesAPI.Data;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace KejaHUnt_PropertiesAPI.Repositories.Implementation
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PropertyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Property> CreatePropertyAsync(Property property)
        {
            await _dbContext.Properties.AddAsync(property);
            await _dbContext.SaveChangesAsync();
            return property;
        }

        public async Task<Property?> DeleteAync(int id)
        {
            var existingProperty = await _dbContext.Properties.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProperty != null)
            {
                _dbContext.Units.RemoveRange(existingProperty.Units);
                _dbContext.Properties.Remove(existingProperty);
                await _dbContext.SaveChangesAsync();
                return existingProperty;
            }
            return null;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _dbContext.Properties.Include(x => x.Units).ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _dbContext.Properties.Include(x => x.Units).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Property?> UpdateAsync(int id, UpdatePropertyRequestDto property)
        {
            var existingProperty= await _dbContext.Properties.Include(x => x.Units)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (existingProperty == null)
            {
                return null;
            }

            _dbContext.Entry(existingProperty).CurrentValues.SetValues(property);

            _dbContext.Units.RemoveRange(existingProperty.Units);

            existingProperty.Units = property.Units.Select(u => new Unit
            {
                Price = u.Price,
                Type= u.Type,
                Bathrooms = u.Bathrooms,
                Size = u.Size,
                NoOfUnits = u.NoOfUnits,
            }).ToList();

            await _dbContext.SaveChangesAsync();

            return existingProperty;

        }
    }
}
