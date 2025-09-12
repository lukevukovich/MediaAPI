namespace MediaAPI.Models.Stremio
{
    public class Manifest
    {
        public string Id { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Logo { get; set; } = null;
        public List<string> Types { get; set; } = [];
        public List<string> Resources { get; set; } = [];
        public List<string> IdPrefixes { get; set; } = [];
        public List<string> ExtraSupported { get; set; } = [];
        public List<Catalog> Catalogs { get; set; } = [];
    }
}