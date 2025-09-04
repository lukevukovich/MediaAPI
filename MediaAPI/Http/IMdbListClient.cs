namespace MdbListApi.Http
{
    public interface IMdbListClient
    {
        Task<HttpResponseMessage> GetListAsync(string owner, string slug, CancellationToken cancellationToken = default);
    }
}