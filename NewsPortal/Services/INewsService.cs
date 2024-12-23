using NewsPortal.Models;

namespace NewsPortal.Services
{
    public interface INewsService
    {
       Task<List<Article>> GetNewsAsync(string category);
    }
}
