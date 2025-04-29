using Microsoft.AspNetCore.Mvc;

namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class CreateUnitsJsonDto
    {
        [FromForm(Name = "imageFile")]
        public List<IFormFile> ImageFiles { get; set; } // One image per unit, same order
        [FromForm(Name = "units")]
        public string Units { get; set; }
    }
}
