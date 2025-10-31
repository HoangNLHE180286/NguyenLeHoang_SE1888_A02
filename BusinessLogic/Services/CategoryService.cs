using BusinessLogic.DTOs;
using BusinessLogic.DTOs.Requests;
using BusinessLogic.DTOs.Response;
using BusinessLogic.Rules;
using BusinessLogic.Validation;
using Core.Enums;
using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class CategoryService {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly CategoryValidator _categoryValidator;
        private readonly CategoryRules _categoryRules;


        public CategoryService(ICategoryRepository categoryRepository, CategoryValidator categoryValidator, CategoryRules categoryRules, INewsArticleRepository newsArticleRepository) {
            _categoryRepository = categoryRepository;
            _categoryValidator = categoryValidator;
            _categoryRules = categoryRules;
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task AddCategoryAsync(CreateCategoryRequest createCategoryRequest) {
            _categoryValidator.ValidateForCreate(createCategoryRequest);
            await _categoryRules.CheckForCreate(createCategoryRequest);

            var category = new Category {
                CategoryName = createCategoryRequest.CategoryName,
                CategoryDesciption = createCategoryRequest.CategoryDescription,
                IsActive = true,
                ParentCategoryId = (createCategoryRequest.ParentCategoryId == null || createCategoryRequest.ParentCategoryId == 0) ? null : (short)createCategoryRequest.ParentCategoryId,
            };

            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<IEnumerable<CategoryResponse>> SearchCategoryAsync(string? keyword, Status? status) {
            bool? isActive = status == null ? null : status == Status.Active;
            var categories = await _categoryRepository.SearchCategoryAsync(keyword, isActive);

            return categories.Select(c => {
                var articleQuantity = _newsArticleRepository.CountNewsArticlesByCategoryIdAsync(c.CategoryId).Result;
                return new CategoryResponse {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDesciption,
                    Status = c.IsActive != null && (bool)c.IsActive ? Status.Active : Status.Inactive,
                    ParentCategory = c.ParentCategory == null ? null : new CategoryResponse {
                        CategoryId = c.ParentCategory.CategoryId,
                        CategoryName = c.ParentCategory.CategoryName,
                        CategoryDescription = c.ParentCategory.CategoryDesciption,
                        Status = c.ParentCategory.IsActive != null && (bool)c.ParentCategory.IsActive ? Status.Active : Status.Inactive
                    },
                    ArticleCount = articleQuantity
                };
            }).ToList();
        }

        public async Task<CategoryResponse> GetCategoryByIdAsync(int categoryId) {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            return new CategoryResponse {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDesciption,
                Status = category.IsActive != null && (bool)category.IsActive ? Status.Active : Status.Inactive,
                ParentCategory = category.ParentCategory == null ? null : new CategoryResponse {
                    CategoryId = category.ParentCategory.CategoryId,
                    CategoryName = category.ParentCategory.CategoryName,
                    CategoryDescription = category.ParentCategory.CategoryDesciption,
                    Status = category.ParentCategory.IsActive != null && (bool)category.ParentCategory.IsActive ? Status.Active : Status.Inactive
                }
            };
        }

        public async Task UpdateCategoryAsync(UpdateCategoryRequest updateCategoryRequest) {
            _categoryValidator.ValidateForUpdate(updateCategoryRequest);
            await _categoryRules.CheckForUpdate(updateCategoryRequest);

            await _categoryRepository.UpdateCategoryAsync(new Category {
                CategoryId = (short)updateCategoryRequest.CategoryId,
                CategoryName = updateCategoryRequest.CategoryName,
                CategoryDesciption = updateCategoryRequest.CategoryDescription,
                ParentCategoryId = (updateCategoryRequest.ParentCategoryId == null || updateCategoryRequest.ParentCategoryId == 0) ? null : (short)updateCategoryRequest.ParentCategoryId
            });
        }

        public async Task DeleteCategoryAsync(int categoryId) {
            await _categoryRules.CheckForDelete(categoryId);
            await _categoryRepository.DeleteCategoryAsync(categoryId);
        }

        public async Task ChangeStatusAsync(int categoryId, Status status) {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            category.IsActive = status == Status.Active;

            await _categoryRepository.UpdateCategoryAsync(category);
        }

    }
}
