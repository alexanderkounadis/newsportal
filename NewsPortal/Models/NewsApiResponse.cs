using System.Text.Json.Serialization;

namespace NewsPortal.Models
{
    public class NewsApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("articles")]
        public List<Article> Articles { get; set; }
       
    }
}
