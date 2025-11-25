using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MediaAPI.Services;
using MediaAPI.Models.MdbList;
using MediaAPI.Models.Tmdb;
using MediaAPI.Models;
using Moq;
using System.Text.Json;
using FluentAssertions;

namespace MediaAPI.Tests.Integration;

public class MediaControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private MdbList? _mdbList;
    private TmdbPoster? _tmdbPoster;
    private Mock<IMdbListService> _mdbListServiceMock = new();
    private Mock<ITmdbService> _tmdbServiceMock = new();

    public MediaControllerTests(WebApplicationFactory<Program> factory)
    {
        _mdbList = new MdbList
        {
            Owner = "testowner",
            Name = "Test List",
            Items = new List<MdbItem>
                {
                    new MdbItem { ImdbId = "tt1234567", MediaType = "movie", Title = "Test Movie", ReleaseYear = 2020 },
                    new MdbItem { ImdbId = "tt7654321", MediaType = "movie", Title = "Test Movie 2", ReleaseYear = 2021 },
                    new MdbItem { ImdbId = "tt2345678", MediaType = "movie", Title = "Test Movie 3", ReleaseYear = 2019 }
                }
        };

        _tmdbPoster = new TmdbPoster
        {
            ImdbId = "tt1234567",
            PosterPath = "/path/to/poster.jpg"
        };

        _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<MdbList>
            {
                Success = true,
                Value = _mdbList
            });

        _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<TmdbPoster>
            {
                Success = true,
                Value = _tmdbPoster
            });

        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_ => _mdbListServiceMock.Object);
                services.AddSingleton(_ => _tmdbServiceMock.Object);
            });
        });

        _client = customFactory.CreateClient();
    }

    [Fact]
    public async Task GetList_ReturnsMdbList()
    {
        var response = await _client.GetAsync("/media/list/testowner/testlist");
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = await response.Content.ReadAsStringAsync();
        var mdbList = JsonSerializer.Deserialize<MdbList>(content, options);
        Assert.NotNull(mdbList);
        mdbList.Should().BeEquivalentTo(_mdbList);
    }

    [Fact]
    public async Task GetList_ReturnsProblem()
    {
        _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<MdbList>
            {
                Success = false,
                ErrorMessage = "Not Found",
                StatusCode = 404
            });

        var response = await _client.GetAsync("/media/list/invalidowner/invalidlist");
        Assert.Equal(404, (int)response.StatusCode);
    }

    [Fact]
    public async Task GetPoster_ReturnsPoster()
    {
        var response = await _client.GetAsync("/media/poster/tt1234567");
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = await response.Content.ReadAsStringAsync();
        var tmdbPoster = JsonSerializer.Deserialize<TmdbPoster>(content, options);
        Assert.NotNull(tmdbPoster);
        tmdbPoster.Should().BeEquivalentTo(_tmdbPoster);
    }

    [Fact]
    public async Task GetPoster_ReturnsProblem()
    {
        _tmdbServiceMock.Setup(s => s.ProxyPosterPathAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<TmdbPoster>
            {
                Success = false,
                ErrorMessage = "Not Found",
                StatusCode = 404
            });

        var response = await _client.GetAsync("/media/poster/invalidid");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(404, (int)response.StatusCode);
        Assert.Contains("Not Found", content);
    }
}