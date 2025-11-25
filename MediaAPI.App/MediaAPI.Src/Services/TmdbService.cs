using MediaAPI.Http;
using MediaAPI.Models;
using MediaAPI.Models.Tmdb;
using MediaAPI.Options;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MediaAPI.Services;

public interface ITmdbService
{
    Task<ProxyResult<ITmdbDetails>> ProxyDetailsAsync(string imdb_id, CancellationToken cancellationToken = default);
    Task<ProxyResult<TmdbPoster>> ProxyPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default);
}

public class TmdbService : ITmdbService
{
    private readonly ITmdbClient _client;
    private readonly TmdbOptions _options;

    public TmdbService(ITmdbClient client, IOptions<TmdbOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task<ProxyResult<ITmdbDetails>> ProxyDetailsAsync(string imdb_id, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetDetailsAsync(imdb_id, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return new ProxyResult<ITmdbDetails>
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = (int)response.StatusCode
            };
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        TmdbResponse? tmdbResponse;
        try
        {
            tmdbResponse = await JsonSerializer.DeserializeAsync<TmdbResponse>(stream, options, cancellationToken);
        } catch (JsonException ex)
        {
            return new ProxyResult<ITmdbDetails>
            {
                Success = false,
                ErrorMessage = $"Failed to deserialize TMDB JSON: {ex.Message}",
                StatusCode = 500
            };
        }

        if (tmdbResponse?.MovieResults != null && tmdbResponse.MovieResults.Count > 0)
        {
            return new ProxyResult<ITmdbDetails>
            {
                Success = true,
                Value = tmdbResponse.MovieResults.FirstOrDefault(),
                StatusCode = 200
            };
        }
        else if (tmdbResponse?.TvResults != null && tmdbResponse.TvResults.Count > 0)
        {
            return new ProxyResult<ITmdbDetails>
            {
                Success = true,
                Value = tmdbResponse.TvResults.FirstOrDefault(),
                StatusCode = 200
            };
        }
        else
        {
            return new ProxyResult<ITmdbDetails>
            {
                Success = false,
                ErrorMessage = $"No TMDB details found for IMDB ID {imdb_id}",
                StatusCode = 404
            };
        }
    }

    public async Task<ProxyResult<TmdbPoster>> ProxyPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetDetailsAsync(imdb_id, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            return new ProxyResult<TmdbPoster>
            {
                Success = false,
                ErrorMessage = error,
                StatusCode = (int)response.StatusCode
            };
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        TmdbResponse? tmdbResponse;
        try
        {
            tmdbResponse = await JsonSerializer.DeserializeAsync<TmdbResponse>(stream, options, cancellationToken);
        } catch (JsonException ex)
        {
            return new ProxyResult<TmdbPoster>
            {
                Success = false,
                ErrorMessage = $"Failed to deserialize TMDB JSON: {ex.Message}",
                StatusCode = 500
            };
        }
        
        var posterPath = tmdbResponse?.MovieResults?.FirstOrDefault()?.PosterPath ?? tmdbResponse?.TvResults?.FirstOrDefault()?.PosterPath;
        if (string.IsNullOrEmpty(posterPath))
        {
            return new ProxyResult<TmdbPoster>
            {
                Success = false,
                ErrorMessage = $"Poster not found for IMDB ID {imdb_id}",
                StatusCode = 404
            };
        }

        var tmdbPoster = new TmdbPoster
        {
            ImdbId = imdb_id,
            PosterPath = $"{_options.PosterBaseUrl}{posterPath}"
        };
        return new ProxyResult<TmdbPoster>
        {
            Success = true,
            Value = tmdbPoster,
            StatusCode = 200
        };
    }
}