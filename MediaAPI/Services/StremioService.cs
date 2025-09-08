using MediaAPI.Models;
using MediaAPI.Models.Stremio;

namespace MediaAPI.Services
{
    public class StremioService : IStremioService
    {
        private readonly IMdbListService _mdbListService;

        public StremioService(IMdbListService mdbListService)
        {
            _mdbListService = mdbListService;
        }

        public async Task<ProxyResult<CatalogMetas>> ProxyCatalogMetasAsync(string owner, string name, string? filter, Dictionary<string, List<string>>? filterMap, CancellationToken cancellationToken = default)
        {
            if (filter is not null && filter.Contains("="))
                filter = filter.Split("=")[1];

            var result = await _mdbListService.ProxyListAsync(owner, name, poster: false, cancellationToken: cancellationToken);
            if (!result.Success)
            {
                return new ProxyResult<CatalogMetas>
                {
                    Success = false,
                    ErrorMessage = result.ErrorMessage,
                    StatusCode = result.StatusCode
                };
            }

            var mdbList = result.Value;
            if (mdbList == null || mdbList.Items == null)
            {
                return new ProxyResult<CatalogMetas>
                {
                    Success = false,
                    ErrorMessage = "Failed to retrieve MDBList items.",
                    StatusCode = 500
                };
            }

            var filteredItems = string.IsNullOrWhiteSpace(filter) || filterMap is null || !filterMap.TryGetValue(filter, out List<string>? value) ? mdbList.Items
                : mdbList.Items.Where(item => value.Any(f => item.Title!.Contains(f, StringComparison.OrdinalIgnoreCase)));

            await _mdbListService.AddPosterUrls(filteredItems.ToList(), cancellationToken);

            var catalog = new CatalogMetas
            {
                Metas = [.. filteredItems.Select(item => new CatalogItem
                {
                    Id = item.ImdbId ?? string.Empty,
                    Type = "movie",
                    Name = item.Title ?? string.Empty,
                    Poster = item.PosterPath ?? string.Empty,
                    Year = item.ReleaseYear ?? 0
                })]
            };

            return new ProxyResult<CatalogMetas>
            {
                Success = true,
                Value = catalog,
                StatusCode = 200
            };
        }
    }
}