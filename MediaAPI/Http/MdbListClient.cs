namespace MdbListApi.Http
{
    public class MdbListClient : IMdbListClient
    {
        private readonly HttpClient _httpClient;

        public MdbListClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetListAsync(string owner, string name, CancellationToken cancellationToken = default)
        {
            var requestUrl = $"lists/{owner}/{name}/json";
            return await _httpClient.GetAsync(requestUrl, cancellationToken);
        }
    }
}