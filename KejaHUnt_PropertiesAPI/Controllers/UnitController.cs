using AutoMapper;
using KejaHUnt_PropertiesAPI.Migrations;
using KejaHUnt_PropertiesAPI.Models.Domain;
using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Implementation;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KejaHUnt_PropertiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public UnitController(IUnitRepository unitRepository, IImageRepository imageRepository, IMapper mapper)
        {
            _unitRepository = unitRepository;
            _imageRepository = imageRepository;
            _mapper = mapper;
        }

        // POST: {apibaseurl}/api/unit
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromForm] CreateUnitsJsonDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Deserialize the units JSON string into a list of CreateUnitRequestDto
            List<CreateUnitRequestDto> unitDtos;
            try
            {
                unitDtos = JsonConvert.DeserializeObject<List<CreateUnitRequestDto>>(request.Units);
            }
            catch (Exception)
            {
                return BadRequest("Invalid units JSON format.");
            }

            if (request.ImageFiles == null || request.ImageFiles.Count != unitDtos.Count)
            {
                return BadRequest("Number of images must match number of units.");
            }

            var unitsToSave = new List<Unit>();

            // 2. Loop through units and images in parallel
            for (int i = 0; i < unitDtos.Count; i++)
            {
                var unitDto = unitDtos[i];
                var unitEntity = _mapper.Map<Unit>(unitDto);

                var image = request.ImageFiles[i];
                if (image != null)
                {
                    Guid? documentId = await _imageRepository.Upload(image);
                    unitEntity.DocumentId = documentId;
                }

                await _unitRepository.CreateUnitAsync(unitEntity);
                unitsToSave.Add(unitEntity);
            }

            return Ok(_mapper.Map<List<UnitDto>>(unitsToSave));
        }

        // GET: {apibaseurl}/api/unit
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var units = await _unitRepository.GetAllAsync();

            return Ok(_mapper.Map<List<UnitDto>>(units));
        }

        // GET: {apibaseurl}/api/unit/{id}
        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetUnitByIdAsync([FromRoute] long id)
        {
            var unit = await _unitRepository.GetUnitByIdAsync(id);

            if (unit == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<UnitDto>>(unit));
        }

        // GET: {apibaseurl}/api/Unit/property/{propertyId}
        [HttpGet]
        [Route("property/{propertyId:long}")]
        public async Task<IActionResult> GetUnitByPropertyIdAsync([FromRoute] long propertyId)
        {
            var unit = await _unitRepository.GetUnitsByPropertyIdAsync(propertyId);

            if (unit == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<UnitDto>>(unit));
        }

        // PUT: {apibaseurl}/api/unit/{id}
        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdateUnitById([FromRoute] long id, [FromForm] UpdateUnitJsonDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Deserialize the unit JSON
            UpdateUnitRequestDto unitDto;
            try
            {
                unitDto = JsonConvert.DeserializeObject<UpdateUnitRequestDto>(request.Unit);
            }
            catch (Exception)
            {
                return BadRequest("Invalid unit JSON format.");
            }

            // 2. Get existing unit
            var existingUnit = await _unitRepository.GetUnitByIdAsync(id);
            if (existingUnit == null)
                return NotFound($"Unit with ID {id} not found.");

            // 3. Handle image update if a file is included
            Guid? documentIdToUse = existingUnit.DocumentId;
            if (request.ImageFile != null)
            {
                documentIdToUse = (documentIdToUse != null && documentIdToUse != Guid.Empty)
                    ? await _imageRepository.Edit(documentIdToUse.Value, request.ImageFile)
                    : await _imageRepository.Upload(request.ImageFile);
            }

            // 4. Update unit fields
            existingUnit = _mapper.Map<UpdateUnitRequestDto, Unit>(unitDto, existingUnit);
            existingUnit.DocumentId = documentIdToUse;
            existingUnit.Id = id;

            // 5. Save to DB
            await _unitRepository.UpdateAsync(existingUnit);

            return Ok(_mapper.Map<UnitDto>(existingUnit));
        }

        // DELETE: {apibaseurl}/api/unit/{id}
        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteUnitById([FromRoute] long id)
        {
            // 1. Get the unit
            var existingUnit = await _unitRepository.GetUnitByIdAsync(id);
            if (existingUnit == null)
                return NotFound($"Unit with ID {id} not found.");

            // 2. Delete associated image if present
            //if (existingUnit.DocumentId != null && existingUnit.DocumentId != Guid.Empty)
            //{
            //    await _imageRepository.Delete(existingUnit.DocumentId.Value);
            //}

            // 3. Delete unit
            await _unitRepository.DeleteAync(id);

            //return deleted unit
            return Ok(_mapper.Map<UnitDto>(existingUnit));
        }

    }
}
