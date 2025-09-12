using Moq;
using MediaAPI.Services;
using MediaAPI.Models;
using MediaAPI.Models.MdbList;
using MediaAPI.Models.Stremio;
using System.Collections.Specialized;

namespace MediaAPI.Tests.Services
{
    public class StremioServiceTests
    {
        private Mock<IMdbListService> _mdbListServiceMock = new();

        [Fact]
        public async Task ProxyCatalogMetasAsync_InvalidFilter_ReturnsError()
        {
            var mockMdbListService = new Mock<IMdbListService>();
            var stremioService = new StremioService(mockMdbListService.Object);
            var filterMap = new OrderedDictionary { { "action", new List<string> { "Action" } }, { "comedy", new List<string> { "Comedy" } } };

            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", "invalid", filterMap, null);

            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Invalid filter value: invalid", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_MdbListServiceError_ReturnsError()
        {
            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = false, ErrorMessage = "Service error", StatusCode = 500 });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", null, null, null);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Service error", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_NullMdbList_ReturnsError()
        {
            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = null });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", null, null, null);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Failed to retrieve MDBList items.", result.ErrorMessage);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_ValidFilter_ReturnsFilteredItems()
        {
            var items = new List<MdbItem>
            {
                new MdbItem { ImdbId = "tt001", Title = "Action Movie", ReleaseYear = 2020 },
                new MdbItem { ImdbId = "tt002", Title = "Comedy Movie", ReleaseYear = 2021 },
                new MdbItem { ImdbId = "tt003", Title = "Drama Movie", ReleaseYear = 2019 }
            };
            var mdbList = new MdbList { Items = items };

            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = mdbList });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var filterMap = new OrderedDictionary { { "action", new List<string> { "Action" } }, { "comedy", new List<string> { "Comedy" } } };

            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", "action", filterMap, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Metas);
            Assert.Equal("tt001", result.Value.Metas[0].Id);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_FilterAll_ReturnsFilteredItems()
        {
            var items = new List<MdbItem>
            {
                new MdbItem { ImdbId = "tt001", Title = "Action Movie", ReleaseYear = 2020 },
                new MdbItem { ImdbId = "tt002", Title = "Comedy Movie", ReleaseYear = 2021 },
                new MdbItem { ImdbId = "tt003", Title = "Drama Movie", ReleaseYear = 2019 }
            };
            var mdbList = new MdbList { Items = items };

            var filterMap = new OrderedDictionary { { "action", new List<string> { "Action" } }, { "comedy", new List<string> { "Comedy" } } };

            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = mdbList });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", "all", filterMap, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Metas.Count);
            var ids = result.Value.Metas.Select(m => m.Id).ToList();
            Assert.Contains("tt001", ids);
            Assert.Contains("tt002", ids);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_NoFilter_ReturnsAllItems()
        {
            var items = new List<MdbItem>
            {
                new MdbItem { ImdbId = "tt001", Title = "Action Movie", ReleaseYear = 2020 },
                new MdbItem { ImdbId = "tt002", Title = "Comedy Movie", ReleaseYear = 2021 },
                new MdbItem { ImdbId = "tt003", Title = "Drama Movie", ReleaseYear = 2019 }
            };
            var mdbList = new MdbList { Items = items };

            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = mdbList });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", null, null, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Metas.Count);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_NoFilterMap_ReturnsAllItems()
        {
            var items = new List<MdbItem>
            {
                new MdbItem { ImdbId = "tt001", Title = "Action Movie", ReleaseYear = 2020 },
                new MdbItem { ImdbId = "tt002", Title = "Comedy Movie", ReleaseYear = 2021 },
                new MdbItem { ImdbId = "tt003", Title = "Drama Movie", ReleaseYear = 2019 }
            };
            var mdbList = new MdbList { Items = items };

            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = mdbList });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", "action", null, null);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Metas.Count);
        }

        [Fact]
        public async Task ProxyCatalogMetasAsync_SortBy_ReturnsSortedItems()
        {
            var items = new List<MdbItem>
            {
                new MdbItem { ImdbId = "tt001", Title = "B Movie", ReleaseYear = 2020 },
                new MdbItem { ImdbId = "tt002", Title = "A Movie", ReleaseYear = 2021 },
                new MdbItem { ImdbId = "tt003", Title = "C Movie", ReleaseYear = 2019 }
            };
            var mdbList = new MdbList { Items = items };

            _mdbListServiceMock.Setup(s => s.ProxyListAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ProxyResult<MdbList> { Success = true, Value = mdbList });

            var stremioService = new StremioService(_mdbListServiceMock.Object);
            var result = await stremioService.ProxyCatalogMetasAsync("owner", "name", null, null, CatalogSortEnum.NameAscending);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Metas.Count);
            Assert.Equal("tt002", result.Value.Metas[0].Id); // A Movie
            Assert.Equal("tt001", result.Value.Metas[1].Id); // B Movie
            Assert.Equal("tt003", result.Value.Metas[2].Id); // C Movie
        }
    }
}