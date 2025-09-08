using System.Text.Json.Serialization;

namespace MediaAPI.Models
{
    public class MdbItem
    {
        [JsonPropertyName("imdb_id")]
        public string? ImdbId { get; set; } = string.Empty;
        public string? MediaType { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        [JsonPropertyName("release_year")]
        public int? ReleaseYear { get; set; }
        [JsonPropertyName("poster_url")]
        public string? PosterPath { get; set; }
    }
}