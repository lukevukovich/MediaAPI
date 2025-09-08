using MediaAPI.Http;
using MediaAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<ProxyResult<MdbList>> ProxyListAsync(string owner, string name, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetListAsync(owner, name, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = error
                };
            }

            var stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            if (stringResponse.Contains("empty or private list", StringComparison.OrdinalIgnoreCase))
            {
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = "MDBList is empty or does not exist."
                };
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var deserializedList = await JsonSerializer.DeserializeAsync<List<MdbItem>>(stream, options, cancellationToken);
            var itemList = deserializedList?.Where(item => item.ImdbId is not null).ToList() ?? new List<MdbItem>();

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

            if (itemList != null && itemList.Any(x =>
                !string.IsNullOrWhiteSpace(x.ImdbId) ||
                !string.IsNullOrWhiteSpace(x.Title)))
            {
                var mdbList = new MdbList
                {
                    Name = name,
                    Owner = owner,
                    Items = itemList
                };
                return new ProxyResult<MdbList>
                {
                    Success = true,
                    Value = mdbList
                };
            }
            else
            {
                return new ProxyResult<MdbList>
                {
                    Success = false,
                    ErrorMessage = "MDBList could not be retrieved."
                };
            }
        }
    }
}