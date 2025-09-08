using MediaAPI.Models;

namespace MediaAPI.Services
{
    public interface ITmdbService
    {
        Task<ProxyResult<TmdbPoster>> ProxyPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default);
    }
}