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

        public PropertyController(IPropertyRepository propertyRepository, IMapper mapper, IImageRepository imageRepository)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
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
        public async Task<IActionResult> UpdatePropertyById([FromRoute] int id, [FromForm] UpdatePropertyRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Deserialize Units
            List<UpdateUnitRequestDto> unitDtos;
            try
            {
                unitDtos = JsonConvert.DeserializeObject<List<UpdateUnitRequestDto>>(request.Units);
            }
            catch (Exception)
            {
                return BadRequest("Invalid units JSON format.");
            }

            // Manually assign Unit ImageFiles because ASP.NET Core can't bind arrays of files automatically
            for (int i = 0; i < unitDtos.Count; i++)
            {
                var unitImageKey = $"Units[{i}].ImageFile";
                if (Request.Form.Files.Any(f => f.Name == unitImageKey))
                {
                    var file = Request.Form.Files.First(f => f.Name == unitImageKey);
                    unitDtos[i].ImageFile = file;
                }
            }

            Guid? documentIdToUse = request.DocumentId;

            if (request.ImageFile != null)
            {
                // Handle property image upload or edit
                if (request.ImageFile != null)
                {
                    if (documentIdToUse != Guid.Empty && documentIdToUse != null)
                    {
                        documentIdToUse = await _imageRepository.Edit(documentIdToUse.Value, request.ImageFile);
                    }
                    else
                    {
                        documentIdToUse = await _imageRepository.Upload(request.ImageFile);
                    }
                }
            }

            // Fetch the property from database
            var existingProperty = await _propertyRepository.GetPropertyByIdAsync(id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            // Update property fields
            existingProperty.Name = request.Name;
            existingProperty.Location = request.Location;
            existingProperty.Type = request.Type;
            existingProperty.DocumentId = documentIdToUse;

            // Clear old units and add new ones
            existingProperty.Units.Clear();
            foreach (var unitDto in unitDtos)
            {
                Guid? unitDocumentId = unitDto.DocumentId;

                if (unitDto.ImageFile != null)
                {
                    if (unitDocumentId != null && unitDocumentId != Guid.Empty)
                    {
                        unitDocumentId = await _imageRepository.Edit(unitDocumentId.Value, unitDto.ImageFile);
                    }
                    else
                    {
                        unitDocumentId = await _imageRepository.Upload(unitDto.ImageFile);
                    }
                }

                var unit = new Unit
                {
                    Price = unitDto.Price,
                    Type = unitDto.Type,
                    Bathrooms = unitDto.Bathrooms,
                    Size = unitDto.Size,
                    PropertyId = existingProperty.Id,
                    NoOfUnits = unitDto.NoOfUnits,
                    DocumentId = unitDocumentId
                };
                existingProperty.Units.Add(unit);
            }

            // Save changes
            await _propertyRepository.UpdateAsync(existingProperty);

            // Prepare response
            var response = new PropertyDto
            {
                Id = existingProperty.Id,
                Name = existingProperty.Name,
                Location = existingProperty.Location,
                Type = existingProperty.Type,
                DocumentId = existingProperty.DocumentId,
                Units = existingProperty.Units.Select(unit => new UnitDto
                {
                    Price = unit.Price,
                    Type = unit.Type,
                    Bathrooms = unit.Bathrooms,
                    Size = unit.Size,
                    NoOfUnits = unit.NoOfUnits,
                    DocumentId = unit.DocumentId
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
