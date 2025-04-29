using AutoMapper;
using Azure.Core;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Implementation;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KejaHUnt_PropertiesAPI.Controllers
{
    [Route("api/property")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;
        private readonly IImageRepository _imageRepository;
        private readonly IUnitRepository _unitRepository;

        public PropertyController(IPropertyRepository propertyRepository, IMapper mapper, IImageRepository imageRepository, IUnitRepository unitRepository)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
            _unitRepository = unitRepository;
        }

        // POST: {apibaseurl}/api/property
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromForm] CreatePropertyRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            List<CreateUnitRequestDto> unitDtos;
            try
            {
                unitDtos = JsonConvert.DeserializeObject<List<CreateUnitRequestDto>>(request.Units);
            }
            catch (Exception)
            {
                return BadRequest("Invalid units JSON format.");
            }

            // Upload image and get DocumentId
            Guid documentId = await _imageRepository.Upload(request.ImageFile);

            // Map the main property (excluding units)
            var property = _mapper.Map<Property>(request);
            property.DocumentId = documentId;
            property.Units = new List<Unit>();

            // Map and add each unit
            foreach (var unitDto in unitDtos)
            {
                var unit = _mapper.Map<Unit>(unitDto);
                property.Units.Add(unit);
            }

            // Save to database
            await _propertyRepository.CreatePropertyAsync(property);

            // Map back to DTO for response
            return Ok(_mapper.Map<PropertyDto>(property));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var properties = await _propertyRepository.GetAllAsync();

            return Ok(_mapper.Map<List<PropertyDto>>(properties));
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetPropertyByIdAsync([FromRoute] long id)
        {
            var property = await _propertyRepository.GetPropertyByIdAsync(id);

            if (property == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PropertyDto>(property));
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdatePropertyById([FromRoute] long id, [FromForm] UpdatePropertyRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Deserialize Units with safe fallback
            List<UpdateUnitRequestDto> unitDtos = new();
            if (!string.IsNullOrWhiteSpace(request.Units))
            {
                try
                {
                    unitDtos = JsonConvert.DeserializeObject<List<UpdateUnitRequestDto>>(request.Units);
                }
                catch (Exception)
                {
                    return BadRequest("Invalid units JSON format.");
                }
            }

            // Assign files to corresponding units (if any)
            for (int i = 0; i < unitDtos.Count; i++)
            {
                var unitImageKey = $"Units[{i}].ImageFile";
                if (Request.Form.Files.Any(f => f.Name == unitImageKey))
                {
                    var file = Request.Form.Files.First(f => f.Name == unitImageKey);
                    unitDtos[i].ImageFile = file;
                }
            }

            // Fetch existing property
            var existingProperty = await _propertyRepository.GetPropertyByIdAsync(id);
            if (existingProperty == null)
                return NotFound();

            // Upload/edit property image if provided
            if (request.ImageFile != null)
            {
                existingProperty.DocumentId = (request.DocumentId != null && request.DocumentId != Guid.Empty)
                    ? await _imageRepository.Edit(request.DocumentId.Value, request.ImageFile)
                    : await _imageRepository.Upload(request.ImageFile);
            }

            // Update property fields
            existingProperty.Name = request.Name;
            existingProperty.Location = request.Location;
            existingProperty.Type = request.Type;

            // Process units: update existing or add new
            foreach (var unitDto in unitDtos)
            {
                var existingUnit = existingProperty.Units.FirstOrDefault(u => u.Id == unitDto.Id);

                Guid? unitDocumentId = unitDto.DocumentId;

                if (unitDto.ImageFile != null)
                {
                    unitDocumentId = (unitDocumentId != null && unitDocumentId != Guid.Empty)
                        ? await _imageRepository.Edit(unitDocumentId.Value, unitDto.ImageFile)
                        : await _imageRepository.Upload(unitDto.ImageFile);
                }

                if (existingUnit != null)
                {
                    // Use UpdateAsync method
                    var updatedUnit = new Unit
                    {
                        Id = unitDto.Id,
                        Price = unitDto.Price,
                        Type = unitDto.Type,
                        Bathrooms = unitDto.Bathrooms,
                        Size = unitDto.Size,
                        NoOfUnits = unitDto.NoOfUnits,
                        PropertyId = existingProperty.Id,
                        DocumentId = unitDocumentId
                    };

                    await _unitRepository.UpdateAsync(updatedUnit);
                }
                else
                {
                    // Add new unit
                    var newUnit = _mapper.Map<Unit>(unitDto);
                    newUnit.DocumentId = unitDocumentId;
                    existingProperty.Units.Add(newUnit);
                }
            }

            // Save property updates
            await _propertyRepository.UpdateAsync(existingProperty);

            return Ok(_mapper.Map<PropertyDto>(existingProperty));
        }



        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteDiaryEntry([FromRoute] long id)
        {
            var deletedProperty = await _propertyRepository.DeleteAync(id);

            if (deletedProperty == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PropertyDto>(deletedProperty));

        }

    

}
    }
