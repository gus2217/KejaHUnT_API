using KejaHUnt_PropertiesAPI.Models.Dto;
using KejaHUnt_PropertiesAPI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KejaHUnt_PropertiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImageController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var documentId = await _imageRepository.Upload(file);
                return Ok(new { documentId });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        [Route("{documentId:guid}")]
        public async Task<IActionResult> GetFileById(Guid documentId)
        {
            if (documentId == Guid.Empty)
                return BadRequest("Invalid document ID.");

            try
            {
                // Fetch file details from the repository
                var fileResponse = await _imageRepository.GetByDocumentIdAsync(documentId);

                if (fileResponse == null)
                    return NotFound(new { error = "File not found." });

                // Return the file response as it is with the Base64 string
                return Ok(new FileHandlerResponse
                {
                    FileName = fileResponse.FileName,
                    Extension = fileResponse.Extension,
                    CreatedAt = fileResponse.CreatedAt,
                    CreatedBy = fileResponse.CreatedBy,
                    Base64 = fileResponse.Base64
                });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPut]
        [Route("{documentId:guid}")]
        public async Task<IActionResult> EditFile(Guid documentId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Edit the file and get the updated DocumentId
                var updatedDocumentId = await _imageRepository.Edit(documentId, file);
                return Ok(new { documentId = updatedDocumentId, message = "File updated successfully." });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
