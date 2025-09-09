namespace MediaAPI.Models.Stremio
{
    public class Catalog
    {
        public string Type { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Extra> Extra { get; set; } = [];
        public List<string>? ExtraRequired { get; set; } = null;
    }
}