using Microsoft.AspNetCore.Mvc;
using NewsPortal.Services;

namespace NewsPortal.Controllers
{
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [Route("umbraco/api/news/getitems/{documentTypeName}")]
        public async Task<IActionResult> GetItems(string documentTypeName)
        {

            if (string.IsNullOrWhiteSpace(documentTypeName))
            {
                return BadRequest("Category cannot be null or empty.");
            }

            try
            {
                var articles = await _newsService.GetNewsAsync(documentTypeName);

                if (articles == null || !articles.Any())
                {
                    return NotFound($"No articles found for the category: {documentTypeName}");
                }
                return Ok(articles);
            }
            catch (HttpRequestException httpEx)
            {
                // Handle issues related to HTTP requests
                return StatusCode(StatusCodes.Status503ServiceUnavailable, $"Error fetching data from the external API: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                // Generic error handling
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
                }

        }
    }
