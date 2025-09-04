using MdbListApi.Http;
using MdbListApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

namespace MdbListApi.Services
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

        public async Task<IResult> ProxyListAsync(string owner, string name, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetListAsync(owner, name, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                return Results.Problem(title: "MDBList error", detail: error, statusCode: (int)response.StatusCode);
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var deserializedList = await JsonSerializer.DeserializeAsync<List<MdbItem>>(stream, options, cancellationToken);
            var itemList = deserializedList?.Where(item => item.ImdbId is not null).ToList();
            var list = new MdbList
            {
                Name = name,
                Owner = owner,
                Items = itemList ?? new List<MdbItem>()
            };

            var semaphore = new SemaphoreSlim(5); // allow 5 concurrent requests
            var tasks = list.Items
                .Where(item => !string.IsNullOrEmpty(item.ImdbId) && item.ImdbId.StartsWith("tt"))
                .Select(async item =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        var result = await _service.ProxyPosterPathAsync(item.ImdbId!, cancellationToken);
                        var posterPath = result is Ok<TmdbPoster> ok ? ok.Value?.PosterPath : null;
                        if (posterPath is not null)
                            item.PosterPath = posterPath;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks);

            return list is not null ? Results.Ok(list) : Results.NotFound();
        }
    }
}