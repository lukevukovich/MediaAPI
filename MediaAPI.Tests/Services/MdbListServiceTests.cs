using MediaAPI.Services;
using Moq;
using MediaAPI.Tests.Http;
using System.Text.Json;
using MediaAPI.Models.MdbList;
using MediaAPI.Models.Tmdb;
using MediaAPI.Models;

namespace MediaAPI.Tests.Services
{
    public class MdbListServiceTests
    {
        private Mock<ITmdbService> _tmdbServiceMock = new();

        [Fact]
        public async Task ProxyListAsync_ReturnsMdbList()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("[{\"imdb_id\":\"tt1234567\"}, {\"imdb_id\":\"tt7654321\"}]")
            };

            var mdbListClient = MdbListClientTests.CreateClient(fakeResponse);
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            var result = await mdbListService.ProxyListAsync("testowner", "Test List", poster: false);
            var list = result.Value;

            Assert.True(result.Success);
            Assert.NotNull(list);
            Assert.Equal(2, list.Items.Count);
            Assert.Equal("tt1234567", list.Items[0].ImdbId);
            Assert.Equal("tt7654321", list.Items[1].ImdbId);
            Assert.Equal("Test List", list.Name);
            Assert.Equal("testowner", list.Owner);
            Assert.NotEmpty(list.Items);
        }

        [Fact]
        public async Task ProxyListAsync_HttpError_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Internal Server Error")
            };

            var mdbListClient = MdbListClientTests.CreateClient(fakeResponse);
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            var result = await mdbListService.ProxyListAsync("testowner", "Error List", poster: false);

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Internal Server Error", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyListAsync_EmptyOrPrivateList_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("{\"error\":\"empty or private list\"}")
            };

            var mdbListClient = MdbListClientTests.CreateClient(fakeResponse);
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            var result = await mdbListService.ProxyListAsync("testowner", "Empty List", poster: false);

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("MDBList is empty or does not exist.", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyListAsync_CannotDeserializeJson_ReturnsError()
        {
            var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                // Invalid JSON, id should be imdb_id
                Content = new StringContent("{\"id\":\"tt1234567\"}")
            };

            var mdbListClient = MdbListClientTests.CreateClient(fakeResponse);
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            var result = await mdbListService.ProxyListAsync("testowner", "Invalid List", poster: false);
            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.Equal(500, result.StatusCode);
            Assert.Contains("Failed to deserialize MDBList JSON", result.ErrorMessage);
        }

        [Fact]
        public async Task AddPosterUrls_AddsPosterUrlsToItems()
        {
            var fakeList = new MdbList
            {
                Name = "Poster List",
                Owner = "posterowner",
                Items = new List<MdbItem>
                {
                    new MdbItem { ImdbId = "tt1234567" },
                    new MdbItem { ImdbId = "tt7654321" }
                }
            };

            var response1 = new ProxyResult<TmdbPoster>
            {
                Success = true,
                Value = new TmdbPoster { ImdbId = "tt1234567", PosterPath = "/path/to/poster1.jpg" },
                StatusCode = 200
            };
            var response2 = new ProxyResult<TmdbPoster>
            {
                Success = true,
                Value = new TmdbPoster { ImdbId = "tt7654321", PosterPath = "/path/to/poster2.jpg" },
                StatusCode = 200
            };

            _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync("tt1234567", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response1);
            _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync("tt7654321", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response2);

            var mdbListClient = MdbListClientTests.CreateClient(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            await mdbListService.AddPosterUrls(fakeList.Items);

            Assert.Equal("/path/to/poster1.jpg", fakeList.Items[0].PosterPath);
            Assert.Equal("/path/to/poster2.jpg", fakeList.Items[1].PosterPath);
        }

        [Fact]
        public async Task AddPosterUrls_SkipsItemsWithoutImdbId()
        {
            var fakeList = new MdbList
            {
                Name = "Partial Poster List",
                Owner = "partialowner",
                Items = new List<MdbItem>
                {
                    new MdbItem { ImdbId = "tt1234567" },
                    new MdbItem { ImdbId = null }, // No IMDb ID
                    new MdbItem { ImdbId = "tt7654321" }
                }
            };

            var response1 = new ProxyResult<TmdbPoster>
            {
                Success = true,
                Value = new TmdbPoster { ImdbId = "tt1234567", PosterPath = "/path/to/poster1.jpg" },
                StatusCode = 200
            };
            var response2 = new ProxyResult<TmdbPoster>
            {
                Success = true,
                Value = new TmdbPoster { ImdbId = "tt7654321", PosterPath = "/path/to/poster2.jpg" },
                StatusCode = 200
            };

            _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync("tt1234567", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response1);
            _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync("tt7654321", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response2);

            var mdbListClient = MdbListClientTests.CreateClient(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            var mdbListService = new MdbListService(mdbListClient, _tmdbServiceMock.Object);

            await mdbListService.AddPosterUrls(fakeList.Items);

            Assert.Equal("/path/to/poster1.jpg", fakeList.Items[0].PosterPath);
            Assert.Null(fakeList.Items[1].PosterPath);
            Assert.Equal("/path/to/poster2.jpg", fakeList.Items[2].PosterPath);
        }
    }
}