using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public interface ICategoryRepository {
        Task<Category?> GetCategoryByIdAsync(int? categoryId);
        Task<Category?> GetCategoryByNameAsync(string categoryName);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<Category>> SearchCategoryAsync(string? keyword, bool? isActive);
    }
}
