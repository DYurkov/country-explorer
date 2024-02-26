using CountryExplorer.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CountryExplorer.Infrastructure
{
    public class CountryCache : ICountryCache
    {
        private readonly IMemoryCache _cache;
        private const string AllCountriesCacheKey = "allCountries";
        private const string CountryCacheKey = "countryByName_";
        private TimeSpan CacheDuration => TimeSpan.FromHours(2);

        public CountryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<CountrySummary> GetAllCountries()
        {
            _cache.TryGetValue<IEnumerable<CountrySummary>>(AllCountriesCacheKey, out var countries);
            return countries;
        }

        public void SetAllCountries(IEnumerable<CountrySummary> countries)
        {
            _cache.Set(AllCountriesCacheKey, countries, CacheDuration);
        }

        public CountryDetail GetCountryByName(string name)
        {
            var cacheKey = $"{CountryCacheKey}{name}";
            _cache.TryGetValue<CountryDetail>(cacheKey, out var country);
            return country;
        }

        public void SetCountryByName(string name, CountryDetail country)
        {
            var cacheKey = $"{CountryCacheKey}{name}";
            _cache.Set(cacheKey, country, CacheDuration);
        }
    }

}
