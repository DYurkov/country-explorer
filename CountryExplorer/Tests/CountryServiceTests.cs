using CountryExplorer.Services;
using Moq.Protected;
using Moq;
using System.Net;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using CountryExplorer.Infrastructure;
using CountryExplorer.Models;
using CountryExplorer.Models.Exceptions;

namespace CountryExplorer.Tests
{
    public class CountryServiceTests
    {
        private readonly Mock<ICountryCache> _mockCountryCache;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly CountryService _countryService;

        public CountryServiceTests()
        {
            _mockCountryCache = new Mock<ICountryCache>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://restcountries.com/v3.1"),
            };

            _countryService = new CountryService(_httpClient, _mockCountryCache.Object);
        }

        [Fact]
        public async Task GetAllCountriesAsync_ReturnsCachedCountries_IfCacheIsNotEmpty()
        {
            // Arrange
            var expectedCountries = new List<CountrySummary> 
            { 
                new CountrySummary { Name = new CountryName { Common = "Country1" } } 
            };
            _mockCountryCache.Setup(m => m.GetAllCountries()).Returns(expectedCountries);

            // Act
            var result = await _countryService.GetAllCountriesAsync();

            // Assert
            Assert.Equal(expectedCountries, result);
            _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAllCountriesAsync_FetchesFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            var expectedApiResponse = "[{\"name\":{\"common\":\"Country1\"}}]";
            var expectedCountries = new List<CountrySummary> { new CountrySummary { Name = new CountryName { Common = "Country1" } } };
            _mockCountryCache.Setup(m => m.GetAllCountries()).Returns(() => null);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains("all")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedApiResponse)
                });

            // Act
            var result = await _countryService.GetAllCountriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(expectedCountries.Select(c => c.Name.Common), result.Select(c => c.Name.Common));
            _mockCountryCache.Verify(m => m.SetAllCountries(It.IsAny<IEnumerable<CountrySummary>>()), Times.Once());
        }

        [Fact]
        public async Task GetCountryByNameAsync_ReturnsCachedCountry_IfCacheIsNotEmpty()
        {
            // Arrange
            var expectedCountry = new CountryDetail { Name = new CountryName { Common = "Country1" } };
            _mockCountryCache.Setup(m => m.GetCountryByName("Country1")).Returns(expectedCountry);

            // Act
            var result = await _countryService.GetCountryByNameAsync("Country1");

            // Assert
            Assert.Equal(expectedCountry, result);
            _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetCountryByNameAsync_FetchesFromApi_WhenCacheIsEmpty()
        {
            // Arrange
            var countryName = "Country1";
            var expectedApiResponse = "[{\"name\":{\"common\":\"Country1\"}}]";
            var expectedCountry = new CountryDetail { Name = new CountryName { Common = "Country1" } };
            _mockCountryCache.Setup(m => m.GetCountryByName(countryName)).Returns(() => null);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains(countryName)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedApiResponse)
                });

            // Act
            var result = await _countryService.GetCountryByNameAsync(countryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCountry.Name.Common, result.Name.Common);
            _mockCountryCache.Verify(m => m.SetCountryByName(countryName, It.IsAny<CountryDetail>()), Times.Once());
        }
        [Fact]
        public async Task GetAllCountriesAsync_ThrowsException_WhenApiReturnsError()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains("all")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => _countryService.GetAllCountriesAsync());
        }

        [Fact]
        public async Task GetCountryByNameAsync_ThrowsException_WhenApiReturnsError()
        {
            // Arrange
            var countryName = "Country1";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.AbsolutePath.Contains(countryName)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => _countryService.GetCountryByNameAsync(countryName));
        }

        [Fact]
        public async Task GetAllCountriesAsync_ThrowsException_WhenNetworkErrorOccurs()
        {
            // Arrange
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => _countryService.GetAllCountriesAsync());
        }

        [Fact]
        public async Task GetCountryByNameAsync_ThrowsException_WhenNetworkErrorOccurs()
        {
            // Arrange
            var countryName = "Country1";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            await Assert.ThrowsAsync<ExternalApiException>(() => _countryService.GetCountryByNameAsync(countryName));
        }


    }


}
