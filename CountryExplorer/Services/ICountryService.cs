using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using CountryExplorer.Models;

namespace CountryExplorer.Services
{
    public interface ICountryService
    {
        Task<IEnumerable<CountrySummary>> GetAllCountriesAsync();
        Task<CountryDetail> GetCountryByNameAsync(string name);
    }

}
