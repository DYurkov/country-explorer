using System.Text.Json.Serialization;

namespace CountryExplorer.Models
{
    public class Currency
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
}
