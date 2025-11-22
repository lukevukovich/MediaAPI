using System.Text.Json.Serialization;

namespace MediaAPI.Models.Tmdb;

public class TmdbResponse
{
    [JsonPropertyName("movie_results")]
    public List<TmdbPoster> MovieResults { get; set; } = [];
    [JsonPropertyName("tv_results")]
    public List<TmdbPoster> TvResults { get; set; } = [];
}