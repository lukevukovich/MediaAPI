using MediaAPI.Models;
using MediaAPI.Models.Stremio;
using MediaAPI.Models.MdbList;
using System.Collections.Specialized;
using System.Collections;

namespace MediaAPI.Services
{
    public class StremioService : IStremioService
    {
        private readonly IMdbListService _mdbListService; 

        public StremioService(IMdbListService mdbListService)
        {
            _mdbListService = mdbListService;
        }

        public async Task<ProxyResult<CatalogMetas>> ProxyCatalogMetasAsync(string owner, string name, string? filter, OrderedDictionary? filterMap, CatalogSortEnum? sortBy, CancellationToken cancellationToken = default)
        {
            if (filter is not null && filter.Contains('='))
                filter = filter.Split('=')[1];
            
            if (filterMap is not null && !filterMap.Contains(filter ?? string.Empty) && !string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                return new ProxyResult<CatalogMetas>
                {
                    Success = false,
                    ErrorMessage = $"Invalid filter value: {filter}",
                    StatusCode = 400
                };
            }

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

            var filteredItems = Enumerable.Empty<MdbItem>();
            if (filterMap is not null && string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                var orderedKeys = filterMap.Cast<DictionaryEntry>()
                    .Select(entry => (string)entry.Key)
                    .ToList();

                filteredItems = mdbList.Items
                    .Where(item => filterMap.Cast<DictionaryEntry>()
                        .SelectMany(entry => (List<string>)entry.Value!)
                        .Any(f => item.Title?.Contains(f, StringComparison.OrdinalIgnoreCase) == true))
                    .OrderBy(item =>
                    {
                        var matchIndex = orderedKeys.FindIndex(k =>
                        {
                            var values = (List<string>)filterMap[k]!;
                            return values.Any(f => item.Title?.Contains(f, StringComparison.OrdinalIgnoreCase) == true);
                        });
                        return matchIndex == -1 ? int.MaxValue : matchIndex;
                    });
            }
            else if (filterMap is not null && !string.IsNullOrWhiteSpace(filter))
            {
                List<string>? value = null;
                value = filterMap[filter!] as List<string>;
                filteredItems = string.IsNullOrWhiteSpace(filter) || filterMap is null || value == null
                    ? mdbList.Items
                    : mdbList.Items.Where(item => value.Any(f => item.Title!.Contains(f, StringComparison.OrdinalIgnoreCase)));
            }
            else
                filteredItems = mdbList.Items;

            await _mdbListService.AddPosterUrls([.. filteredItems], cancellationToken);

            if (sortBy.HasValue && !string.Equals(filter, "all", StringComparison.OrdinalIgnoreCase))
            {
                filteredItems = sortBy.Value switch
                {
                    CatalogSortEnum.NameAscending => filteredItems.OrderBy(item => item.Title),
                    CatalogSortEnum.NameDescending => filteredItems.OrderByDescending(item => item.Title),
                    CatalogSortEnum.YearAscending => filteredItems.OrderBy(item => item.ReleaseYear ?? int.MaxValue),
                    CatalogSortEnum.YearDescending => filteredItems.OrderByDescending(item => item.ReleaseYear ?? int.MinValue),
                    _ => filteredItems
                };
            }

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