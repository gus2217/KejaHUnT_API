using AutoMapper;
using Azure.Core;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KejaHUnt_PropertiesAPI.Controllers
{
    [Route("api/property")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public PropertyController(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        // POST: {apibaseurl}/api/property
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreatePropertyRequestDto request)
        {
            // Map DTO to domain model
            var property = _mapper.Map<Property>(request);
            

            await _propertyRepository.CreatePropertyAsync(property);

            return Ok(_mapper.Map<PropertyDto>(property));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();

            return Ok(_mapper.Map<List<PropertyDto>>(properties));
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetPropertyByIdAsync([FromRoute] int id)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PropertyDto>(property));
        }

        // PUT: {apibaseurl}/api/property/{id}
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePropertyById([FromRoute] int id, UpdatePropertyRequestDto request)
        {

            var updatedProperty = await _propertyRepository.UpdateAsync(id, request);

            if (updatedProperty == null)
            {
                return NotFound();
            }

            var response = new PropertyDto
            {
                Id = updatedProperty.Id,
                Name = updatedProperty.Name,
                Location = updatedProperty.Location,
                Type = updatedProperty.Type,
                Units = updatedProperty.Units.Select(unit => new UnitDto
                {
                    Price = unit.Price,
                    Type = unit.Type,
                    Bathrooms = unit.Bathrooms,
                    Size = unit.Size,
                    NoOfUnits = unit.NoOfUnits
                }).ToList()
            };

            return Ok(response);

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteDiaryEntry([FromRoute] int id)
        {
            var deletedProperty = await _propertyRepository.DeleteAync(id);

            if (deletedProperty == null)
            {
                return NotFound();
            }

            var response = new PropertyDto
            {
                Id = deletedProperty.Id,
                Name = deletedProperty.Name,
                Location = deletedProperty.Location,
                Type = deletedProperty.Type,
                Units = deletedProperty.Units.Select(unit => new UnitDto
                {
                    Price = unit.Price,
                    Type = unit.Type,
                    Bathrooms = unit.Bathrooms,
                    Size = unit.Size,
                    NoOfUnits = unit.NoOfUnits
                }).ToList()
            };

            return Ok(response);

        }

    

}
    }
