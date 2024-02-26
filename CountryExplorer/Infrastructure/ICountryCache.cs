using CountryExplorer.Models;

namespace CountryExplorer.Infrastructure
{
    public interface ICountryCache
    {
        IEnumerable<CountrySummary> GetAllCountries();
        void SetAllCountries(IEnumerable<CountrySummary> countries);
        CountryDetail GetCountryByName(string name);
        void SetCountryByName(string name, CountryDetail country);
    }
}
