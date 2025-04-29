using KejaHUnt_PropertiesAPI.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class UpdatePropertyRequestDto
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public Guid? DocumentId { get; set; }
        [FromForm(Name = "imageFile")]
        public IFormFile? ImageFile { get; set; } // Attach the file here
        [FromForm(Name = "units")]
        public string? Units { get; set; }
    }
}
