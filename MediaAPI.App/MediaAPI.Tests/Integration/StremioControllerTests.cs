using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MediaAPI.Services;
using MediaAPI.Models.Stremio;
using MediaAPI.Models;
using Moq;
using System.Text.Json;
using FluentAssertions;
using System.Collections.Specialized;

namespace MediaAPI.Tests.Integration;

public class StremioControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private Mock<IStremioService> _stremioServiceMock = new();
    private CatalogMetas? _catalogMetas;

    public StremioControllerTests(WebApplicationFactory<Program> factory)
    {
        _catalogMetas = new CatalogMetas
        {
            Metas = new List<CatalogItem>
            {
                new CatalogItem { Id = "tt1234567", Type = "movie", Name = "Test Movie", Poster = "/path/to/poster1.jpg", Year = 2020 },
                new CatalogItem { Id = "tt7654321", Type = "movie", Name = "Test Movie 2", Poster = "/path/to/poster2.jpg", Year = 2021 },
                new CatalogItem { Id = "tt2345678", Type = "movie", Name = "Test Movie 3", Poster = "/path/to/poster3.jpg", Year = 2019 }
            }
        };

        _stremioServiceMock.Setup(s => s.ProxyCatalogMetasAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<OrderedDictionary?>(), It.IsAny<CatalogSortEnum?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<CatalogMetas>
            {
                Success = true,
                Value = _catalogMetas
            });

        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_ => _stremioServiceMock.Object);
            });
        });

        _client = customFactory.CreateClient();
    }

    [Fact]
    public async Task GetManifest_ReturnsStremioAddonManifest()
    {
        var response = await _client.GetAsync("/stremio/manifest.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var manifest = JsonSerializer.Deserialize<Manifest>(content, options);

        Assert.NotNull(manifest);
        Assert.Equal("stremio.luke.catalog", manifest.Id);
        Assert.Equal("Luke's Catalogs", manifest.Name);
    }

    [Fact]
    public async Task GetSlasherCatalog_ReturnsCatalogMetas()
    {
        var response = await _client.GetAsync("/stremio/catalog/movie/slasher/all.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catalogMetas = JsonSerializer.Deserialize<CatalogMetas>(content, options);

        Assert.NotNull(catalogMetas);
        catalogMetas.Should().BeEquivalentTo(_catalogMetas);
    }

    [Fact]
    public async Task GetSlasherCatalog_ReturnsProblem()
    {
        _stremioServiceMock.Setup(s => s.ProxyCatalogMetasAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<OrderedDictionary?>(), It.IsAny<CatalogSortEnum?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<CatalogMetas>
            {
                Success = false,
                ErrorMessage = "Failed to fetch catalog",
                StatusCode = 500
            });

        var response = await _client.GetAsync("/stremio/catalog/movie/slasher/all.json");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(500, (int)response.StatusCode);
        Assert.Contains("Failed to fetch catalog", content);
    }

    [Fact]
    public async Task GetHorrorCatalog_ReturnsCatalogMetas()
    {
        var response = await _client.GetAsync("/stremio/catalog/movie/horror/all.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catalogMetas = JsonSerializer.Deserialize<CatalogMetas>(content, options);

        Assert.NotNull(catalogMetas);
        catalogMetas.Should().BeEquivalentTo(_catalogMetas);
    }

    [Fact]
    public async Task GetHorrorCatalog_ReturnsProblem()
    {
        _stremioServiceMock.Setup(s => s.ProxyCatalogMetasAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<OrderedDictionary?>(), It.IsAny<CatalogSortEnum?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<CatalogMetas>
            {
                Success = false,
                ErrorMessage = "Failed to fetch catalog",
                StatusCode = 500
            });

        var response = await _client.GetAsync("/stremio/catalog/movie/horror/all.json");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(500, (int)response.StatusCode);
        Assert.Contains("Failed to fetch catalog", content);
    }

    [Fact]
    public async Task GetGangsterCatalog_ReturnsCatalogMetas()
    {
        var response = await _client.GetAsync("/stremio/catalog/movie/gangster.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catalogMetas = JsonSerializer.Deserialize<CatalogMetas>(content, options);

        Assert.NotNull(catalogMetas);
        catalogMetas.Should().BeEquivalentTo(_catalogMetas);
    }

    [Fact]
    public async Task GetGangsterCatalog_ReturnsProblem()
    {
        _stremioServiceMock.Setup(s => s.ProxyCatalogMetasAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<OrderedDictionary?>(), It.IsAny<CatalogSortEnum?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProxyResult<CatalogMetas>
            {
                Success = false,
                ErrorMessage = "Failed to fetch catalog",
                StatusCode = 500
            });

        var response = await _client.GetAsync("/stremio/catalog/movie/gangster.json");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal(500, (int)response.StatusCode);
        Assert.Contains("Failed to fetch catalog", content);
    }
}