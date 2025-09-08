using MediaAPI.Models.MdbList;
using MediaAPI.Models;

namespace MediaAPI.Services
{
    public interface IMdbListService
    {
        Task<ProxyResult<MdbList>> ProxyListAsync(string owner, string name, bool poster = true, CancellationToken cancellationToken = default);
        Task AddPosterUrls(List<MdbItem> items, CancellationToken cancellationToken = default);
    }
}