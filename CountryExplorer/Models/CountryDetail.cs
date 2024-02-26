using System.Text.Json.Serialization;

namespace CountryExplorer.Models
{
    public class CountryDetail : CountrySummary
    {
        [JsonPropertyName("flags")]
        public Flag Flags { get; set; }
    }
}
