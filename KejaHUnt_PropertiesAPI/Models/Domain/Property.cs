namespace KejaHUnt_PropertiesAPI.Models.Domain
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
