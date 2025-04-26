namespace KejaHUnt_PropertiesAPI.Models.Domain
{
    public class Image
    {
        public long Id { get; set; }
        public string? Title { get; set; }
        public Guid DocumentId { get; set; }         // From FileHandler
        public string Url { get; set; } = string.Empty; // URL to retrieve image via FileHandler
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
