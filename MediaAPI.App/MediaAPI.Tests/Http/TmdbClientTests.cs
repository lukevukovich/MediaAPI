using MediaAPI.Http;
using MediaAPI.Options;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace MediaAPI.Tests.Http;

public class TmdbClientTests
{
    public static (TmdbClient, Mock<IOptions<TmdbOptions>>) CreateClient(HttpResponseMessage fakeResponse)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(fakeResponse)
            .Verifiable();

        var optionsMock = new Mock<IOptions<TmdbOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new TmdbOptions
        {
            BaseUrl = "https://api.themoviedb.org/3/",
            ApiKey = "test_api_key"
        });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/")
        };
        return (new TmdbClient(httpClient, optionsMock.Object), optionsMock);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsSuccess()
    {
        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("{\"status\":\"ok\"}")
        };

        var (tmdbClient, _) = CreateClient(fakeResponse);
        var response = await tmdbClient.GetDetailsAsync("imdb_id");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"status\":\"ok\"}", content);
    }

    [Fact]
    public async Task GetDetailsAsync_ReturnsError()
    {
        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"status\":\"error\",\"message\":\"Not Found\"}")
        };

        var (tmdbClient, _) = CreateClient(fakeResponse);
        var response = await tmdbClient.GetDetailsAsync("imdb_id");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("{\"status\":\"error\",\"message\":\"Not Found\"}", content);
    }
}