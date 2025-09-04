namespace MdbListApi.Services
{
    public interface IMdbListService
    {
        Task<IResult> ProxyListAsync(string owner, string name, CancellationToken cancellationToken = default);
    }
}