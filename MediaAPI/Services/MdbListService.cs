using MediaAPI.Http;
using MediaAPI.Models;
using MediaAPI.Models.MdbList;
using System.Text.Json;

namespace MediaAPI.Services
{
    public class MdbListService : IMdbListService
    {
        private readonly IMdbListClient _client;
        private readonly ITmdbService _service;

        public MdbListService(IMdbListClient client, ITmdbService service)
        {
            _client = client;
            _service = service;
        }

        public async Task<ProxyResult<MdbList>> ProxyListAsync(string owner, string name, bool poster = true, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetListAsync(owner, name, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = error,
                    StatusCode = (int)response.StatusCode
                };
            }

            var stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            if (stringResponse.Contains("empty or private list", StringComparison.OrdinalIgnoreCase))
            {
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = "MDBList is empty or does not exist.",
                    StatusCode = 404
                };
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            
            List<MdbItem>? deserializedList;
            try
            {
                deserializedList = await JsonSerializer.DeserializeAsync<List<MdbItem>>(stream, options, cancellationToken);
            }
            catch (JsonException ex)
            {
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = $"Failed to deserialize MDBList JSON: {ex.Message}",
                    StatusCode = 500
                };
            }
            var itemList = deserializedList?.Where(item => item.ImdbId is not null).ToList() ?? new List<MdbItem>();

            if (poster)
                await AddPosterUrls(itemList, cancellationToken);

            var mdbList = new MdbList
            {
                Name = name,
                Owner = owner,
                Items = itemList
            };
            return new ProxyResult<MdbList>
            {
                Success = true,
                Value = mdbList,
                StatusCode = 200
            };
        }

        public async Task AddPosterUrls(List<MdbItem> itemList, CancellationToken cancellationToken = default)
        {
            var semaphore = new SemaphoreSlim(5);
            var tasks = itemList
                .Where(item => !string.IsNullOrEmpty(item.ImdbId) && item.ImdbId.StartsWith("tt"))
                .Select(async item =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var result = await _service.ProxyPosterPathAsync(item.ImdbId!, cancellationToken);
                        var posterPath = result.Success ? result.Value?.PosterPath : null;
                        if (posterPath is not null)
                            item.PosterPath = posterPath;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks);
        }
    }
}