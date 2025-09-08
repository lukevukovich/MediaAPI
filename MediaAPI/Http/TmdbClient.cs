using MediaAPI.Options;
using Microsoft.Extensions.Options;

namespace MediaAPI.Http
{
    public class TmdbClient : ITmdbClient
    {
        private readonly HttpClient _httpClient;
        private readonly TmdbOptions _options;

        public TmdbClient(HttpClient httpClient, IOptions<TmdbOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<HttpResponseMessage> GetPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"find/{imdb_id}?api_key={_options.ApiKey}&external_source=imdb_id";
            return await _httpClient.GetAsync(requestUrl, cancellationToken);
        }
    }
}