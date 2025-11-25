using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MediaAPI.Models.Tmdb;

public interface ITmdbDetails
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }
}

public class TmdbMovieDetails : ITmdbDetails
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string? Title { get; set; } = string.Empty;

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("original_title")]
    public string? OriginalTitle { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string? Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; } = string.Empty;

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = new();

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("video")]
    public bool Video { get; set; }

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }
}

public class TmdbTvDetails : ITmdbDetails
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("adult")]
    public bool Adult { get; set; }

    [JsonPropertyName("backdrop_path")]
    public string? BackdropPath { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; set; } = string.Empty;

    [JsonPropertyName("original_name")]
    public string? OriginalName { get; set; } = string.Empty;

    [JsonPropertyName("overview")]
    public string? Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; } = string.Empty;

    [JsonPropertyName("original_language")]
    public string? OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("genre_ids")]
    public List<int> GenreIds { get; set; } = new();

    [JsonPropertyName("popularity")]
    public double Popularity { get; set; }

    [JsonPropertyName("first_air_date")]
    public string? FirstAirDate { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")]
    public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")]
    public int VoteCount { get; set; }

    [JsonPropertyName("origin_country")]
    public List<string> OriginCountry { get; set; } = new();
}