using MdbListApi.Http;
using MdbListApi.Models;
using MdbListApi.Options;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MdbListApi.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly ITmdbClient _client;
        private readonly TmdbOptions _options;

        public TmdbService(ITmdbClient client, IOptions<TmdbOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<IResult> ProxyPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetPosterPathAsync(imdb_id, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                return Results.Problem(title: "Tmdb Poster error", detail: error, statusCode: (int)response.StatusCode);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var tmdbResponse = await JsonSerializer.DeserializeAsync<TmdbResponse>(stream, options, cancellationToken);
            var posterPath = tmdbResponse?.MovieResults?.FirstOrDefault()?.PosterPath ?? tmdbResponse?.TvResults?.FirstOrDefault()?.PosterPath;
            if (string.IsNullOrEmpty(posterPath))
                return Results.Problem(title: "TMDB Poster error", detail: $"Poster not found for IMDB ID {imdb_id}", statusCode: 404);

            var tmdbPoster = new TmdbPoster
            {
                ImdbId = imdb_id,
                PosterPath = $"{_options.PosterBaseUrl}{posterPath}"
            };
            return Results.Ok(tmdbPoster);
        }
    }
}