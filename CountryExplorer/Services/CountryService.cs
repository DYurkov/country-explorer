using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CountryExplorer.Infrastructure;
using CountryExplorer.Models;
using CountryExplorer.Models.Exceptions;
using Polly;

namespace CountryExplorer.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly ICountryCache _countryCache;
        private const string BaseUrl = "https://restcountries.com/v3.1";

        public CountryService(HttpClient httpClient, ICountryCache countryCache)
        {
            _httpClient = httpClient;
            _countryCache = countryCache;
        }

        public async Task<IEnumerable<CountrySummary>> GetAllCountriesAsync()
        {
            var cachedCountries = _countryCache.GetAllCountries();
            if (cachedCountries == null || !cachedCountries.Any())
            {
                try
                {
                    IEnumerable<CountrySummary> countries = await Policy
                        .Handle<Exception>()
                        .RetryAsync(3)
                        .ExecuteAsync(async () =>
                        {
                            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/all");
                            response.EnsureSuccessStatusCode();
                            var content = await response.Content.ReadAsStringAsync();
                            return JsonSerializer.Deserialize<IEnumerable<CountrySummary>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        });

                    _countryCache.SetAllCountries(countries);

                    return countries;
                }
                catch (HttpRequestException e)
                {
                    throw new ExternalApiException("External API error", e);
                }
            }

            return cachedCountries;
        }

        public async Task<CountryDetail> GetCountryByNameAsync(string name)
        {
            var cachedCountry = _countryCache.GetCountryByName(name);
            if (cachedCountry == null)
            {
                try
                {
                    CountryDetail country = await Policy
                        .Handle<Exception>()
                        .RetryAsync(3)
                        .ExecuteAsync(async () =>
                        {
                            HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/name/{name}?fullText=true");
                            response.EnsureSuccessStatusCode();
                            var content = await response.Content.ReadAsStringAsync();
                            var countries = JsonSerializer.Deserialize<IEnumerable<CountryDetail>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            return countries.FirstOrDefault();
                        });

                    if (country != null)
                    {
                        _countryCache.SetCountryByName(name, country);
                    }

                    return country;
                }
                catch (HttpRequestException e)
                {
                    throw new ExternalApiException("External API error", e);
                }
            }

            return cachedCountry;
        }
    }
}
