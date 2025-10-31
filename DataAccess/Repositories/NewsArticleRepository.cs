using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class NewsArticleRepository : INewsArticleRepository {
        private readonly ApplicationDbContext _dbContext;

        public NewsArticleRepository(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<NewsArticle> GetNewsArticleByIdAsync(string newsArticleId) {
            return await _dbContext.NewsArticles.Include(a => a.Category).Include(a => a.Tags).FirstOrDefaultAsync(a => a.NewsArticleId == newsArticleId);
        }

        public async Task<NewsArticle?> GetNewsArticleByTitleAsync(string newsTitle) => await _dbContext.NewsArticles.Include(a => a.Category).Include(a => a.Tags).FirstOrDefaultAsync(a => a.NewsTitle == newsTitle);

        public async Task AddNewsArticleAsync(NewsArticle newsArticle) {
            if (string.IsNullOrEmpty(newsArticle.NewsSource)) {
                newsArticle.NewsSource = "N/A";
            }

            newsArticle.CreatedDate = DateTime.Now;

            var lastId = await this.GetLastIdAsync();
            newsArticle.NewsArticleId = int.Parse(lastId) + 1 + "";

            await _dbContext.NewsArticles.AddAsync(newsArticle);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsArticleAsync(string? keyword, string? author, string? category, bool? isActive, DateTime? from, DateTime? to) {
            keyword = keyword?.ToLower();
            author = author?.ToLower();
            category = category?.ToLower();

            var query = _dbContext.NewsArticles.Include(a => a.Category).Include(a => a.Tags).AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword)) {
                query = query.Where(a => a.NewsTitle.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(author)) {
                query = query.Where(a => a.CreatedBy.AccountName.ToLower().Contains(author));
            }

            if (!string.IsNullOrWhiteSpace(category)) {
                query = query.Where(a => a.Category.CategoryName.ToLower().Contains(category));
            }

            if (isActive.HasValue) {
                query = query.Where(a => a.NewsStatus == isActive.Value);
            }

            if (from.HasValue) {
                query = query.Where(a => a.CreatedDate >= from.Value);
            }

            return await query.ToListAsync();

        }

        public async Task DeleteNewsArticleAsync(string newsArticleId) {
            var existing = await _dbContext.NewsArticles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.NewsArticleId == newsArticleId);
            existing.Tags = null!;
            await _dbContext.SaveChangesAsync();

            await _dbContext.NewsArticles.Where(a => a.NewsArticleId == newsArticleId).ExecuteDeleteAsync();
        }



        public Task UpdateNewsArticleAsync(NewsArticle newsArticle) {
            var existing = _dbContext.NewsArticles.FirstOrDefault(a => a.NewsArticleId == newsArticle.NewsArticleId);

            if (!string.IsNullOrEmpty(newsArticle.NewsTitle)) {
                existing.NewsTitle = newsArticle.NewsTitle;
            }

            if (!string.IsNullOrEmpty(newsArticle.Headline)) {
                existing.Headline = newsArticle.Headline;
            }

            if (!string.IsNullOrEmpty(newsArticle.NewsContent)) {
                existing.NewsContent = newsArticle.NewsContent;
            }

            if (!string.IsNullOrEmpty(newsArticle.NewsSource)) {
                existing.NewsSource = newsArticle.NewsSource;
            } else {
                existing.NewsSource = "N/A";
            }

            if (newsArticle.CategoryId.HasValue) {
                existing.CategoryId = newsArticle.CategoryId.Value;
            }

            if (newsArticle.UpdatedById.HasValue) {
                existing.UpdatedById = newsArticle.UpdatedById.Value;
            }

            existing.Tags = newsArticle.Tags;

            existing.ModifiedDate = DateTime.Now;

            return _dbContext.SaveChangesAsync();
        }

        public Task<int> CountNewsArticlesByAccountIdAsync(int accountId) {
            return _dbContext.NewsArticles.Where(a => a.CreatedById == accountId).CountAsync();
        }

        public Task<int> CountNewsArticlesByCategoryIdAsync(int categoryId) {
            return _dbContext.NewsArticles.Where(a => a.CategoryId == categoryId).CountAsync();
        }

        private async Task<string> GetLastIdAsync() {
            return await _dbContext.NewsArticles
                .OrderByDescending(a => a.NewsArticleId)
                .Select(a => a.NewsArticleId)
                .FirstOrDefaultAsync();
        }
    }
}
