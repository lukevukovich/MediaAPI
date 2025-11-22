namespace MediaAPI.Models.Stremio;

public class CatalogItem
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Poster { get; set; } = string.Empty;
    public int Year { get; set; }
}