namespace MdbListApi.Http
{
    public interface ITmdbClient
    {
        Task<HttpResponseMessage> GetPosterPathAsync(string imdb_id, CancellationToken cancellationToken = default);
    }
}