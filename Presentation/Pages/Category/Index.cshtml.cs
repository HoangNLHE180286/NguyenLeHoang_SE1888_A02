using BusinessLogic.DTOs.Requests;
using BusinessLogic.Services;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;
using Presentation.Models.Params;
using System.Text.Json;
using static Presentation.Models.Params.SearchCategoryParams;

namespace Presentation.Pages.Category {
    public class IndexModel : PageModel {

        private readonly CategoryService _categoryService;

        public IndexModel(CategoryService categoryService) {
            _categoryService = categoryService;
        }

        [BindProperty(SupportsGet = true)]
        public SearchCategoryParams SearchParams { get; set; } = new();

        public List<CategoryData> Categories { get; set; } = new();

        public async Task<IActionResult> OnGetSearchCategoryAsync() {
            var keyword = string.IsNullOrWhiteSpace(SearchParams.Keyword) ? null : SearchParams.Keyword;
            Status? status = SearchParams.Status == null ? null : (Status)Enum.Parse(typeof(Status), SearchParams.Status);

            var categories = await _categoryService.SearchCategoryAsync(keyword, status);

            var list = categories.Select(c => new CategoryData {
                Id = c.CategoryId,
                Name = c.CategoryName,
                Description = c.CategoryDescription,
                Status = c.Status,
                ArticleQuantity = c.ArticleCount,
                Parent = c.ParentCategory == null ? null : new CategoryData {
                    Id = c.ParentCategory.CategoryId,
                    Name = c.ParentCategory.CategoryName,
                    Status = c.ParentCategory.Status,
                    Description = c.ParentCategory.CategoryDescription,
                }
            }).ToList();

            return ViewComponent("CategoryList", list);
        }

        public async Task<IActionResult> OnGetOpenModalAsync(ActionType actionType, int? categoryId) {
            switch (actionType) {
                case ActionType.Create:
                var parents = await _categoryService.SearchCategoryAsync(null, null);
                var categories = parents.Select(c => new CategoryData {
                    Id = c.CategoryId,
                    Name = c.CategoryName,
                    Description = c.CategoryDescription,
                    Status = c.Status
                }).ToList();
                return ViewComponent("CategoryCreateUpdateForm", new { actionType = ActionType.Create, categories = categories });
                case ActionType.Update:
                var category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
                parents = await _categoryService.SearchCategoryAsync(null, null);
                categories = parents.Where(c => c.CategoryId != categoryId.Value).Select(c => new CategoryData {
                    Id = c.CategoryId,
                    Name = c.CategoryName,
                    Description = c.CategoryDescription,
                    Status = c.Status
                }).ToList();
                return ViewComponent("CategoryCreateUpdateForm", new {
                    actionType = ActionType.Update, createUpdateCategoryParams = new CreateUpdateCategoryParams {
                        Id = category.CategoryId,
                        Name = category.CategoryName,
                        Description = category.CategoryDescription,
                        ParentId = category.ParentCategory == null ? null : category.ParentCategory.CategoryId
                    },
                    categories = categories
                });
                case ActionType.Delete:
                category = await _categoryService.GetCategoryByIdAsync(categoryId.Value);
                return ViewComponent("CategoryDeleteForm", category.CategoryName);
                default:
                return null;
            }
        }

        public async Task<IActionResult> OnPostCreateUpdateAsync(ActionType actionType, [FromForm] CreateUpdateCategoryParams createUpdateCategoryParams) {
            if (!ModelState.IsValid) {
                var parents = await _categoryService.SearchCategoryAsync(null, null);
                dynamic categories;
                if (actionType == ActionType.Create) {
                    categories = parents.Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.CategoryDescription,
                        Status = c.Status
                    }).ToList();
                } else {
                    categories = parents.Where(c => c.CategoryId != createUpdateCategoryParams.Id).Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.CategoryDescription,
                        Status = c.Status
                    }).ToList();
                }

                return ViewComponent("CategoryCreateUpdateForm", new {
                    actionType = actionType,
                    createUpdateCategoryParams = createUpdateCategoryParams,
                    categories = categories
                });
            }

            try {
                switch (actionType) {
                    case ActionType.Create:
                    var createParams = new CreateCategoryRequest {
                        CategoryName = createUpdateCategoryParams.Name,
                        CategoryDescription = createUpdateCategoryParams.Description,
                        ParentCategoryId = createUpdateCategoryParams.ParentId
                    };
                    await _categoryService.AddCategoryAsync(createParams);
                    break;
                    case ActionType.Update:
                    var updateParams = new UpdateCategoryRequest {
                        CategoryId = createUpdateCategoryParams.Id.Value,
                        CategoryName = createUpdateCategoryParams.Name,
                        CategoryDescription = createUpdateCategoryParams.Description,
                        ParentCategoryId = createUpdateCategoryParams.ParentId
                    };
                    await _categoryService.UpdateCategoryAsync(updateParams);
                    break;
                }

                return new JsonResult(new { success = true });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("Name", ex.Message);
                Response.StatusCode = 400;
                var parents = await _categoryService.SearchCategoryAsync(null, null);
                dynamic categories;
                if (actionType == ActionType.Create) {
                    categories = parents.Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.CategoryDescription,
                        Status = c.Status
                    }).ToList();
                } else {
                    categories = parents.Where(c => c.CategoryId != createUpdateCategoryParams.Id).Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName,
                        Description = c.CategoryDescription,
                        Status = c.Status
                    }).ToList();
                }

                return ViewComponent("CategoryCreateUpdateForm", new {
                    actionType = actionType,
                    createUpdateCategoryParams = createUpdateCategoryParams,
                    categories = categories
                });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int categoryId) {
            try {
                await _categoryService.DeleteCategoryAsync(categoryId);
                return new JsonResult(new { success = true });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int categoryId, bool isActive) {
            try {
                await _categoryService.ChangeStatusAsync(categoryId, isActive ? Status.Active : Status.Inactive);

                return new JsonResult(new { success = true });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}