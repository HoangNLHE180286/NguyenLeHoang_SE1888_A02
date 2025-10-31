using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public interface INewsArticleRepository {
        Task<IEnumerable<NewsArticle>> SearchNewsArticleAsync(string? keyword, string? author, string? category, bool? isActive, DateTime? from, DateTime? to);
        Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId);
        Task<NewsArticle?> GetNewsArticleByTitleAsync(string newsTitle);
        Task AddNewsArticleAsync(NewsArticle newsArticle);
        Task UpdateNewsArticleAsync(NewsArticle newsArticle);
        Task DeleteNewsArticleAsync(string newsArticleId);
        Task<int> CountNewsArticlesByAccountIdAsync(int accountId);
        Task<int> CountNewsArticlesByCategoryIdAsync(int categoryId);
    }
}
