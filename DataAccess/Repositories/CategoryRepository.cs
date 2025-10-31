using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {

    public class CategoryRepository : ICategoryRepository {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddCategoryAsync(Category category) {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int categoryId) =>
            await _dbContext.Categories.Where(c => c.CategoryId == categoryId).ExecuteDeleteAsync();


        public async Task<Category?> GetCategoryByIdAsync(int? categoryId) {
            if (!categoryId.HasValue) return null;
            return await _dbContext.Categories
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<Category?> GetCategoryByNameAsync(string categoryName) => await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());

        public async Task UpdateCategoryAsync(Category category) {
            var existing = await _dbContext.Categories.FindAsync(category.CategoryId);

            if (!string.IsNullOrEmpty(category.CategoryName)) {
                existing.CategoryName = category.CategoryName;
            }

            if (!string.IsNullOrEmpty(category.CategoryDesciption)) {
                existing.CategoryDesciption = category.CategoryDesciption;
            }

            if (category.IsActive.HasValue) {
                existing.IsActive = category.IsActive.Value;
            }

            if (category.ParentCategoryId.HasValue) {
                existing.ParentCategoryId = category.ParentCategoryId.Value;
            } else {
                existing.ParentCategoryId = null;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> SearchCategoryAsync(string? keyword, bool? isActive) {
            keyword = keyword?.ToLower();

            var query = _dbContext.Categories.AsQueryable();
            if (isActive.HasValue) {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            if (!string.IsNullOrEmpty(keyword)) {
                query = query.Where(c => c.CategoryName.ToLower().Contains(keyword));
            }


            return await query.ToListAsync();
        }
    }
}
