using System.Text.Json.Serialization;

namespace NewsPortal.Models
{
    public class Source
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
