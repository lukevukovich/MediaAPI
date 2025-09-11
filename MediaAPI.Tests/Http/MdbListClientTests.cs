using MediaAPI.Http;
using Moq;
using Moq.Protected;

namespace MediaAPI.Tests.Http
{
    public class MdbListClientTests
    {
        [Fact]
        public async Task GetListAsync_ReturnsSuccess()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"status\":\"ok\"}")
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(fakeResponse)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.mdblist.com/")
            };

            var mdbListClient = new MdbListClient(httpClient);
            var response = await mdbListClient.GetListAsync("owner", "name");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"status\":\"ok\"}", content);
        }

        [Fact]
        public async Task GetListAsync_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"status\":\"error\",\"message\":\"Not Found\"}")
            };

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(fakeResponse)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.mdblist.com/")
            };

            var mdbListClient = new MdbListClient(httpClient);
            var response = await mdbListClient.GetListAsync("owner", "name");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"status\":\"error\",\"message\":\"Not Found\"}", content);
        }
    }
}