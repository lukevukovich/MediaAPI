using System.Text.Json.Serialization;

namespace MediaAPI.Models.Tmdb;

public class TmdbPoster
{
    [JsonPropertyName("imdb_id")]
    public string? ImdbId { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; } = string.Empty;
}