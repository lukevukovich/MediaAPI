using MediaAPI.Options;
using Microsoft.Extensions.Options;

namespace MediaAPI.Http;

public interface ITmdbClient
{
    Task<HttpResponseMessage> GetDetailsAsync(string id, string external_source, CancellationToken cancellationToken = default);
}
    
public class TmdbClient : ITmdbClient
{
    private readonly HttpClient _httpClient;
    private readonly TmdbOptions _options;

    public TmdbClient(HttpClient httpClient, IOptions<TmdbOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<HttpResponseMessage> GetDetailsAsync(string id, string external_source, CancellationToken cancellationToken = default)
    {
        var requestUrl = $"find/{id}?api_key={_options.ApiKey}&external_source={external_source}";
        return await _httpClient.GetAsync(requestUrl, cancellationToken);
    }
}