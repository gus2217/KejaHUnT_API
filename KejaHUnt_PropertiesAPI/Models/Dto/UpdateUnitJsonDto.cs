namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class UpdateUnitJsonDto
    {
            public string Unit { get; set; } // JSON string representing UpdateUnitRequestDto
            public IFormFile? ImageFile { get; set; }
    }
}
