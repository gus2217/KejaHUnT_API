namespace KejaHUnt_PropertiesAPI.Models.Dto
{
    public class FileHandlerResponse
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Base64 { get; set; }
    }
}
