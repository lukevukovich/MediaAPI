namespace MdbListApi.Services
{
    public interface ITmdbService
    {
        Task<IResult> ProxyPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default);
    }
}