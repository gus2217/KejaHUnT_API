using AutoMapper;
using KejaHUnT_TenantAPI.Models.Domain;
using KejaHUnT_TenantAPI.Models.Dto;
using KejaHUnT_TenantAPI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KejaHUnT_TenantAPI.Controllers
{
    [Route("api/tenant")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMapper _mapper;

        public TenantController(ITenantRepository tenantRepository, IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] CreateTenantRequestDto request)
        {
            // Map DTO to domain model
            var tenant = _mapper.Map<Tenant>(request);


            await _tenantRepository.CreateTenantAsync(tenant);

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var tenants = await _tenantRepository.GetAllAsync();

            return Ok(_mapper.Map<List<TenantDto>>(tenants));

        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetPropertyByIdAsync([FromRoute] long id)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TenantDto>(tenant));
        }

        [HttpPut]
        [Route("{id:long}")]
        public async Task<IActionResult> UpdatePropertyById([FromRoute] long id, UpdateTenantRequestDto request)
        {


            var updatedProperty = await _tenantRepository.UpdateAsync(id, request);

            if (updatedProperty == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TenantDto>(updatedProperty));

        }

        [HttpDelete]
        [Route("{id:long}")]
        public async Task<IActionResult> DeleteDiaryEntry([FromRoute] long id)
        {
            var deletedTenant = await _tenantRepository.DeleteAync(id);

            if (deletedTenant == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TenantDto>(deletedTenant));

        }
    }
}
