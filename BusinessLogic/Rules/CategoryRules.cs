using BusinessLogic.DTOs.Requests;
using Core.Enums;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Rules {
    public class CategoryRules {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsArticleRepository _newsArticleRepository;

        public CategoryRules(ICategoryRepository categoryRepository, INewsArticleRepository newsArticleRepository) {
            _categoryRepository = categoryRepository;
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task CheckForCreate(CreateCategoryRequest createCategoryRequest) {
            await this.CheckCategoryAsync(createCategoryRequest.CategoryName, CheckMode.MustNotExist);
        }

        public async Task CheckForUpdate(UpdateCategoryRequest updateCategoryRequest) {

            var currentCategory = await _categoryRepository.GetCategoryByIdAsync(updateCategoryRequest.CategoryId);
            var articleQuantity = await _newsArticleRepository.CountNewsArticlesByCategoryIdAsync(updateCategoryRequest.CategoryId);

            var existingCategory = await _categoryRepository.GetCategoryByNameAsync(updateCategoryRequest.CategoryName);

            if (updateCategoryRequest.CategoryName != currentCategory.CategoryName && existingCategory != null) {
                throw new InvalidOperationException("Category has already existed!");
            }

            if (updateCategoryRequest.ParentCategoryId == currentCategory.CategoryId) {
                throw new InvalidOperationException("Category already has the same parent category!");
            }

            if (articleQuantity > 0 && updateCategoryRequest.ParentCategoryId != currentCategory.ParentCategoryId) {
                throw new Exception("This category has already had some articles. Cannot change its parent!");
            }
        }

        public async Task CheckForDelete(int categoryId) {
            var articleQuantity = await _newsArticleRepository.CountNewsArticlesByCategoryIdAsync(categoryId);
            if (articleQuantity > 0)
                throw new InvalidOperationException("This category has already had some articles. Cannot delete it!");
        }

        private async Task CheckCategoryAsync(string name, CheckMode mode) {
            var category = await _categoryRepository.GetCategoryByNameAsync(name);
            switch (mode) {
                case CheckMode.MustExist:
                if (category == null)
                    throw new InvalidOperationException("Category doesn't exist!");
                break;

                case CheckMode.MustNotExist:
                if (category != null)
                    throw new InvalidOperationException("Category has already existed!");
                break;
            }
        }
    }
}
