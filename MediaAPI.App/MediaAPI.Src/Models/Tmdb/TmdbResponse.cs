using System.Text.Json.Serialization;

namespace MediaAPI.Models.Tmdb;

public class TmdbResponse
{
    [JsonPropertyName("movie_results")]
    public List<TmdbMovieDetails> MovieResults { get; set; } = [];
    [JsonPropertyName("tv_results")]
    public List<TmdbTvDetails> TvResults { get; set; } = [];
}