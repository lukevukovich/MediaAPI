using MediaAPI.Models.Stremio;
using MediaAPI.Models;

namespace MediaAPI.Services
{
    public interface IStremioService
    {
        Task<ProxyResult<CatalogMetas>> ProxyCatalogMetasAsync(string owner, string name, string? filter, Dictionary<string, List<string>>? filterMap, CancellationToken cancellationToken = default);
    }
}