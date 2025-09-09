using MediaAPI.Models.Stremio;
using MediaAPI.Models;
using System.Collections.Specialized;

namespace MediaAPI.Services
{
    public interface IStremioService
    {
        Task<ProxyResult<CatalogMetas>> ProxyCatalogMetasAsync(string owner, string name, string? filter, OrderedDictionary? filterMap, CatalogSortEnum? sortBy, CancellationToken cancellationToken = default);
    }
}