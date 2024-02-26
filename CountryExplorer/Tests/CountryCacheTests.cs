using CountryExplorer.Infrastructure;
using CountryExplorer.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace CountryExplorer.Tests
{
    public class CountryCacheTests
    {
        private readonly Mock<IMemoryCache> _mockMemoryCache;
        private readonly CountryCache _countryCache;
        private MemoryCacheEntryOptions _cacheEntryOptions;
        private object _cacheValue;

        public CountryCacheTests()
        {
            _mockMemoryCache = new Mock<IMemoryCache>();
            _countryCache = new CountryCache(_mockMemoryCache.Object);

            _cacheEntryOptions = new MemoryCacheEntryOptions();
            _cacheValue = null;

            _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns((object key) =>
            {
                return new Mock<ICacheEntry>().Object;
            });
        }

        [Fact]
        public void SetAllCountries_SavesDataCorrectly()
        {
            var mockCacheEntry = new Mock<ICacheEntry>();
            object key = null;
            object value = null;

            _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry.Object)
                .Callback((object k) =>
                {
                    key = k;
                });

            mockCacheEntry.SetupSet(e => e.Value = It.IsAny<object>()).Callback<object>(v =>
            {
                value = v;
            });

            var countries = new List<CountrySummary>
            {
                new CountrySummary { Name = new CountryName { Common = "Country1" } },
                new CountrySummary { Name = new CountryName { Common = "Country2" } }
            };

            _countryCache.SetAllCountries(countries);

            _mockMemoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);
            mockCacheEntry.VerifySet(e => e.Value = It.IsAny<object>(), Times.Once);
            Assert.Equal("allCountries", key);
            Assert.Equal(countries, value);
        }


        [Fact]
        public void GetAllCountries_RetrievesDataCorrectly()
        {
            _mockMemoryCache.Setup(m => m.TryGetValue("allCountries", out _cacheValue)).Returns(true);

            var result = _countryCache.GetAllCountries();

            _mockMemoryCache.Verify(m => m.TryGetValue("allCountries", out _cacheValue), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void GetAllCountries_RetrievesDataCorrectly_WithDataInCache()
        {
            var expectedCountries = new List<CountrySummary>
            {
                new CountrySummary { Name = new CountryName { Common = "Country1" } },
                new CountrySummary { Name = new CountryName { Common = "Country2" } }
            };

            object outValue = expectedCountries;
            _mockMemoryCache.Setup(m => m.TryGetValue("allCountries", out outValue)).Returns(true);

            var result = _countryCache.GetAllCountries();

            _mockMemoryCache.Verify(m => m.TryGetValue("allCountries", out outValue), Times.Once);
            Assert.Equal(expectedCountries, result);
        }

        [Fact]
        public void SetCountryByName_SetsValueInCache()
        {
            var mockCacheEntry = new Mock<ICacheEntry>();
            var countryDetail = new CountryDetail
            {
                Name = new CountryName { Common = "Country1" },
                Capital = new List<string> { "Capital1" }
            };
            var cacheKey = $"countryByName_{countryDetail.Name.Common}";

            _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(mockCacheEntry.Object);

            _countryCache.SetCountryByName(countryDetail.Name.Common, countryDetail);

            _mockMemoryCache.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Once);

            mockCacheEntry.Verify(x => x.Dispose(), Times.Once); // Предполагая, что вы вызываете Dispose после добавления в кеш
        }

        [Fact]
        public void GetCountryByName_RetrievesDataCorrectly_WithDataInCache()
        {
            var expectedCountry = new CountryDetail
            {
                Name = new CountryName { Common = "Country1" },
                Capital = new List<string> { "Capital1" }
            };
            var cacheKey = $"countryByName_{expectedCountry.Name.Common}";

            object outValue = expectedCountry;
            _mockMemoryCache.Setup(m => m.TryGetValue(cacheKey, out outValue)).Returns(true);

            var result = _countryCache.GetCountryByName(expectedCountry.Name.Common);

            _mockMemoryCache.Verify(m => m.TryGetValue(cacheKey, out outValue), Times.Once);
            Assert.Equal(expectedCountry, result);
        }

    }
}
