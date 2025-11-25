using MediaAPI.Tests.Http;
using MediaAPI.Services;
using MediaAPI.Models.Tmdb;
using System.Text.Json;

namespace MediaAPI.Tests.Services;

public class TmdbServiceTests
{
    private enum FakeMediaType
    {
        Movie,
        Tv,
        Empty,
        Null
    }

    private HttpResponseMessage _getFakeDetailsResponseMessage(FakeMediaType media_type)
    {
        var fakeMovieDetails = new TmdbMovieDetails
        {
            Id = "tt1234567",
            Adult = false,
            BackdropPath = "/backdrop.jpg",
            Title = "Fake Movie",
            OriginalLanguage = "en",
            OriginalTitle = "Fake Movie Original",
            Overview = "A fake movie for testing.",
            PosterPath = "/poster.jpg",
            MediaType = "movie",
            GenreIds = new List<int> { 1, 2 },
            Popularity = 10.5,
            ReleaseDate = "2025-01-01",
            Video = false,
            VoteAverage = 8.7,
            VoteCount = 100
        };

        var fakeTvDetails = new TmdbTvDetails
        {
            Id = "tt7654321",
            Adult = false,
            BackdropPath = "/tvbackdrop.jpg",
            Name = "Fake TV Show",
            OriginalName = "Fake TV Show Original",
            Overview = "A fake TV show for testing.",
            PosterPath = "/tvposter.jpg",
            MediaType = "tv",
            OriginalLanguage = "en",
            GenreIds = new List<int> { 3, 4 },
            Popularity = 20.1,
            FirstAirDate = "2024-05-10",
            VoteAverage = 7.5,
            VoteCount = 50,
            OriginCountry = new List<string> { "US" }
        };
        
        string contentString = string.Empty;
        if (media_type == FakeMediaType.Movie)
            contentString = JsonSerializer.Serialize(new { movie_results = new[] { fakeMovieDetails }, tv_results = Array.Empty<object>() });
        else if (media_type == FakeMediaType.Tv)
            contentString = JsonSerializer.Serialize(new { movie_results = Array.Empty<object>(), tv_results = new[] { fakeTvDetails } });
        else if (media_type == FakeMediaType.Empty)
            contentString = "{}";
        else if (media_type == FakeMediaType.Null)
            contentString = "null";

        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(contentString)
        };

        return fakeResponse;
    }

    private HttpResponseMessage _getFakePosterResponseMessage(FakeMediaType media_type)
    {
        var fakeMoviePoster = new TmdbMovieDetails {
            Id = "tt1234567",
            PosterPath = "/path/to/poster.jpg"
        };
        var fakeTvPoster = new TmdbTvDetails
        {
            Id = "tt7654321",
            PosterPath = "/path/to/tvposter.jpg"
        };

        string contentString = string.Empty;
        if (media_type == FakeMediaType.Movie)
            contentString = JsonSerializer.Serialize(new { movie_results = new[] { fakeMoviePoster }, tv_results = Array.Empty<object>() });
        else if (media_type == FakeMediaType.Tv)
            contentString = JsonSerializer.Serialize(new { movie_results = Array.Empty<object>(), tv_results = new[] { fakeTvPoster } });
        else if (media_type == FakeMediaType.Empty)
            contentString = "{}";
        else if (media_type == FakeMediaType.Null)
            contentString = "null";

        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(contentString)
        };

        return fakeResponse;
    }

    [Fact]
    public async Task ProxyDetailsAsync_MovieResults_ReturnsTmdbMovieDetails()
    {
        var fakeResponse = _getFakeDetailsResponseMessage(FakeMediaType.Movie);
        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt1234567");
        var details = result.Value;

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(details);
        Assert.IsType<TmdbMovieDetails>(details);
        var movieDetails = details as TmdbMovieDetails;
        Assert.Equal("tt1234567", movieDetails!.Id);
        Assert.Equal("Fake Movie", movieDetails.Title);
    }

    [Fact]
    public async Task ProxyDetailsAsync_TvResults_ReturnsTmdbTvDetails()
    {
        var fakeResponse = _getFakeDetailsResponseMessage(FakeMediaType.Tv);
        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt7654321");
        var details = result.Value;

        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(details);
        Assert.IsType<TmdbTvDetails>(details);
        var tvDetails = details as TmdbTvDetails;
        Assert.Equal("tt7654321", tvDetails!.Id);
        Assert.Equal("Fake TV Show", tvDetails.Name);
    }

    [Fact]
    public async Task ProxyDetailsAsync_HttpError_ReturnsError()
    {
        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"status\":\"error\",\"message\":\"Not Found\"}")
        };

        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt0000000");

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(404, result.StatusCode);
        Assert.Contains("Not Found", result.ErrorMessage);
    }

    [Fact]
    public async Task ProxyDetailsAsync_CannotDeserializeJson_ReturnsError()
    {
        var fakeResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            // Invalid JSON, missing closing brace
            Content = new StringContent("{")
        };

        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt1234567");

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("Failed to deserialize TMDB JSON", result.ErrorMessage);
    }

    [Fact]
    public async Task ProxyDetailsAsync_NullResults_ReturnsError()
    {
        var fakeResponse = _getFakeDetailsResponseMessage(FakeMediaType.Null);
        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt8888888");

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No TMDB details found for IMDB ID tt8888888", result.ErrorMessage);
    }

    [Fact]
    public async Task ProxyDetailsAsync_EmptyResults_ReturnsError()
    {
        var fakeResponse = _getFakeDetailsResponseMessage(FakeMediaType.Empty);
        var (tmdbClient, tmdbOptions) = TmdbClientTests.CreateClient(fakeResponse);
        var tmdbService = new TmdbService(tmdbClient, tmdbOptions.Object);

        var result = await tmdbService.ProxyDetailsAsync("tt9999999");

        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("No TMDB details found for IMDB ID tt9999999", result.ErrorMessage);
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