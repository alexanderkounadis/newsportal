using Microsoft.Extensions.Caching.Memory;
using NewsPortal.Models;
using Newtonsoft.Json;

namespace NewsPortal.Services
{
    public class NewsService : INewsService
    {
        private const int CacheDurationMinutes = 30;
        private const int MaxArticlesToReturn = 5;
        private readonly string _apiKey = "f73967c13c4b45c886d6bc98d61fce04";

        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public NewsService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<List<Article>> GetNewsAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be null or empty.", nameof(category));

            if (_cache.TryGetValue(category, out IEnumerable<Article> cachedArticles) && cachedArticles != null)
            {
                return cachedArticles.ToList();
            }

            var articles = await FetchArticlesFromApiAsync(category);
            var articlesToReturn = articles.Take(MaxArticlesToReturn).ToList();

            CacheArticles(category, articlesToReturn);

            return articlesToReturn;
        }

        private async Task<List<Article>> FetchArticlesFromApiAsync(string category)
        {
            var url = BuildApiUrl(category);
            ConfigureHttpClientHeaders();
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("An error occurred while sending the request.", ex);
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}.");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var newsResponse = JsonConvert.DeserializeObject<NewsApiResponse>(responseString);

            if (newsResponse?.Articles == null)
            {
                throw new InvalidOperationException("Failed to deserialize API response.");
            }

            return newsResponse.Articles
             .Where(a => !string.IsNullOrWhiteSpace(a.Title) && a.Title != "[Removed]")
             .Select(a => new Article
             {
                 Title = a.Title,
                 Source = a.Source,
                 PublishedAt = a.PublishedAt,
                 Url = a.Url
             })
             .ToList();
        }

        private string BuildApiUrl(string category)
        {
            return $"https://newsapi.org/v2/everything?q={category}&apiKey={_apiKey}";
        }

        private void CacheArticles(string category, List<Article> articles)
        {
            _cache.Set(category, articles, TimeSpan.FromMinutes(CacheDurationMinutes));
        }

        private void ConfigureHttpClientHeaders()
        {
            if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            }
        }
    }
}
