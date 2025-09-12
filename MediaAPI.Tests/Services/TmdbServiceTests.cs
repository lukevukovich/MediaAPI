using MediaAPI.Tests.Http;
using MediaAPI.Services;

namespace MediaAPI.Tests.Services
{
    public class TmdbServiceTests
    {
        private enum FakeMediaType
        {
            Movie,
            Tv,
            Empty,
            Null
        }

        private HttpResponseMessage _getFakePosterResponseMessage(FakeMediaType media_type)
        {
            StringContent? content = null;
            if (media_type == FakeMediaType.Movie)
                content = new StringContent("{\"movie_results\":[{\"imdb_id\":\"tt1234567\",\"poster_path\":\"/path/to/poster.jpg\"}],\"tv_results\":[]}");
            else if (media_type == FakeMediaType.Tv)
                content = new StringContent("{\"movie_results\":[],\"tv_results\":[{\"imdb_id\":\"tt7654321\",\"poster_path\":\"/path/to/tvposter.jpg\"}]}");
            else if (media_type == FakeMediaType.Empty)
                content = new StringContent("{\"movie_results\":[],\"tv_results\":[]}");
            else if (media_type == FakeMediaType.Null)
                content = new StringContent("{\"movie_results\":null,\"tv_results\":null}");

            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = content
            };

            return fakeResponse;
        }

        [Fact]
        public async Task ProxyPosterPathAsync_MovieResults_ReturnsTmdbPoster()
        {
            var fakeResponse = _getFakePosterResponseMessage(FakeMediaType.Movie);
            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt1234567");
            var poster = result.Value;

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(poster);
            Assert.Equal("tt1234567", poster.ImdbId);
            Assert.Equal($"{tmdbOptions.Object.Value.PosterBaseUrl}/path/to/poster.jpg", poster.PosterPath);
        }

        [Fact]
        public async Task ProxyPosterPathAsync_TvResults_ReturnsTmdbPoster()
        {
            var fakeResponse = _getFakePosterResponseMessage(FakeMediaType.Tv);
            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt7654321");
            var poster = result.Value;

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(poster);
            Assert.Equal("tt7654321", poster.ImdbId);
            Assert.Equal($"{tmdbOptions.Object.Value.PosterBaseUrl}/path/to/tvposter.jpg", poster.PosterPath);
        }

        [Fact]
        public async Task ProxyPosterPathAsync_HttpError_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"status\":\"error\",\"message\":\"Not Found\"}")
            };

            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt0000000");

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Not Found", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyPosterPathAsync_CannotDeserializeJson_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                // Invalid JSON, missing closing brace
                Content = new StringContent("{")
            };

            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt1234567");

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Failed to deserialize TMDB JSON", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyPosterPathAsync_NullResults_ReturnsError()
        {
            var fakeResponse = _getFakePosterResponseMessage(FakeMediaType.Null);
            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt8888888");

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Poster not found for IMDB ID tt8888888", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyPosterPathAsync_EmptyResults_ReturnsError()
        {
            var fakeResponse = _getFakePosterResponseMessage(FakeMediaType.Empty);
            var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
            var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

            var result = await tmdbService.ProxyPosterPathAsync("tt9999999");

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Poster not found for IMDB ID tt9999999", result.ErrorMessage);
        }
    }
}