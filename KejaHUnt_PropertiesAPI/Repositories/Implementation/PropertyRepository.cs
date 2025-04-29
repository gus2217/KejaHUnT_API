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

        public async Task<Property?> DeleteAync(long id)
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

        public async Task<Property?> GetPropertyByIdAsync(long id)
        {
            return await _dbContext.Properties.Include(x => x.Units).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Property?> UpdateAsync(Property property)
        {
            var existingProperty = await _dbContext.Properties
                .Include(x => x.Units)
                .FirstOrDefaultAsync(x => x.Id == property.Id);

            if (existingProperty == null)
            {
                return null;
            }

            // Update basic fields
            existingProperty.Name = property.Name;
            existingProperty.Location = property.Location;
            existingProperty.Type = property.Type;
            existingProperty.DocumentId = property.DocumentId;

            // Hold new units in a temporary list
            List<Unit> newUnits = new List<Unit>();

            if (property.Units != null && property.Units.Any())
            {
                foreach (var incomingUnit in property.Units)
                {
                    newUnits.Add(new Unit
                    {
                        Price = incomingUnit.Price,
                        Type = incomingUnit.Type,
                        Bathrooms = incomingUnit.Bathrooms,
                        Size = incomingUnit.Size,
                        NoOfUnits = incomingUnit.NoOfUnits,
                        DocumentId = incomingUnit.DocumentId,
                        PropertyId = existingProperty.Id
                    });
                }
            }


            // Add the new units to the existing property
            existingProperty.Units = newUnits;

            // Save changes
            await _dbContext.SaveChangesAsync();

            return existingProperty;
        }




    }
}
