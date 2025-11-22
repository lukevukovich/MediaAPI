namespace MediaAPI.Options;

public sealed class TmdbOptions
{
    public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
    public string ApiKey { get; set; } = string.Empty;
    public string PosterBaseUrl { get; set; } = "https://image.tmdb.org/t/p/w500";
}