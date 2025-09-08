using MediaAPI.Models;

namespace MediaAPI.Services
{
    public interface IMdbListService
    {
        Task<ProxyResult<MdbList>> ProxyListAsync(string owner, string name, CancellationToken cancellationToken = default);
    }
}