using System.Text.Json.Serialization;

namespace CountryExplorer.Models
{
    public class Flag
    {
        [JsonPropertyName("svg")]
        public string Svg { get; set; }

        [JsonPropertyName("png")]
        public string Png { get; set; }

        [JsonPropertyName("alt")]
        public string Description { get; set; }
    }
}
