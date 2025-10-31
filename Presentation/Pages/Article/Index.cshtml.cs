using BusinessLogic.DTOs.Requests;
using BusinessLogic.Services;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;
using Presentation.Models.Params;
using System.Text.Json;

namespace Presentation.Pages.Article {
    public class IndexModel : PageModel {
        private readonly NewsArticleService _newsArticleService;
        private readonly CategoryService _categoryService;
        private readonly TagService _tagService;

        public IndexModel(NewsArticleService newsArticleService, CategoryService categoryService, TagService tagService) {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        [BindProperty(SupportsGet = true)]
        public SearchArticleParams SearchParams { get; set; } = new();

        public List<CategoryData> Categories { get; set; } = new();

        public async Task OnGetAsync() {
            var categories = await _categoryService.SearchCategoryAsync(null, null);

            Categories = categories.Select(x => new CategoryData() { Id = x.CategoryId, Name = x.CategoryName }).ToList();
        }

        public async Task<IActionResult> OnGetSearchArticleAsync() {
            SearchNewsArticleRequest searchRequest = new() {
                Keyword = string.IsNullOrWhiteSpace(SearchParams.Title) ? null : SearchParams.Title,
                Author = string.IsNullOrWhiteSpace(SearchParams.Author) ? null : SearchParams.Author,
                Category = SearchParams.Category == null ? null : SearchParams.Category,
                Status = SearchParams.Status == null ? null : (Status)Enum.Parse(typeof(Status), SearchParams.Status),
                FromDate = SearchParams.StartDate == null ? null : SearchParams.StartDate,
                ToDate = SearchParams.EndDate == null ? null : SearchParams.EndDate
            };

            var articles = await _newsArticleService.SearchNewsArticleAsync(searchRequest);
            var list = articles.Select(x => new ArticleData() {
                Id = x.NewsArticleId,
                Title = x.NewsTitle!,
                Headline = x.Headline,
                Content = x.NewsContent!,
                Category = x.Category?.CategoryName!,
                Source = x.NewsSource!,
                Status = x.NewsStatus!.Value,
                Author = x.CreatedBy?.AccountName!,
                CreatedAt = x.CreatedDate!.Value,
                ModifiedAt = x.ModifiedDate == null ? null : x.ModifiedDate.Value,
                Modifier = x.UpdatedBy?.AccountName,
                Tags = x.Tags?.Select(y => new TagData() { Id = y.TagId, Name = y.TagName! }).ToList()
            }).ToList();

            return ViewComponent("ArticleList", list);
        }

        public async Task<IActionResult> OnGetOpenModalAsync(ActionType actionType, string? articleId) {
            switch (actionType) {
                // ---------------- CREATE ----------------
                case ActionType.Create: {
                    var categories = await _categoryService.SearchCategoryAsync(null, null);
                    var tags = await _tagService.SearchTagAsync(null);

                    var categoryList = categories.Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName
                    }).ToList();

                    var tagList = tags.Select(t => new TagData {
                        Id = t.TagId,
                        Name = t.TagName!
                    }).ToList();

                    return ViewComponent("ArticleCreateUpdateForm", new {
                        actionType = ActionType.Create,
                        categorieList = categoryList,
                        tagList = tagList
                    });
                }

                // ---------------- UPDATE ----------------
                case ActionType.Update: {
                    var article = await _newsArticleService.GetNewsArticleByIdAsync(articleId!);
                    var categories = await _categoryService.SearchCategoryAsync(null, null);
                    var tags = await _tagService.SearchTagAsync(null);

                    var categoryList = categories.Select(c => new CategoryData {
                        Id = c.CategoryId,
                        Name = c.CategoryName
                    }).ToList();

                    var tagList = tags.Select(t => new TagData {
                        Id = t.TagId,
                        Name = t.TagName!
                    }).ToList();

                    var articleParams = new CreateUpdateArticleParams {
                        Id = article.NewsArticleId.ToString(),
                        Title = article.NewsTitle,
                        Headline = article.Headline,
                        Content = article.NewsContent,
                        CategoryId = article.Category?.CategoryId,
                        Source = article.NewsSource == "Unknown" ? null : article.NewsSource,
                        TagIds = article.Tags?.Select(t => t.TagId).ToList()
                    };

                    return ViewComponent("ArticleCreateUpdateForm", new {
                        actionType = ActionType.Update,
                        categorieList = categoryList,
                        tagList = tagList,
                        createUpdateArticleParams = articleParams
                    });
                }

                // ---------------- DELETE ----------------
                case ActionType.Delete: {
                    var article = await _newsArticleService.GetNewsArticleByIdAsync(articleId!);
                    return ViewComponent("ArticleDeleteForm", article.NewsTitle);
                }

                case ActionType.Duplicate: {
                    var article = await _newsArticleService.GetNewsArticleByIdAsync(articleId!);
                    return ViewComponent("ArticleDuplicateForm", article.NewsTitle);
                }

                default:
                return null!;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(string articleId) {
            try {
                await _newsArticleService.DeleteNewsArticleAsync(articleId);
                return new JsonResult(new { success = true });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDuplicateAsync(string articleId) {
            try {
                await _newsArticleService.DuplicateArticleAsync(articleId);
                return new JsonResult(new { success = true, message = "Article duplicated successfully!" });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }


        public async Task<IActionResult> OnPostCreateUpdateAsync(
    ActionType actionType,
    [FromForm] CreateUpdateArticleParams createUpdateArticleParams) {
            Console.WriteLine(JsonSerializer.Serialize(HttpContext.Session.GetInt32("AccountId")));
            if (!ModelState.IsValid) {
                var categories = await _categoryService.SearchCategoryAsync(null, null);
                var tags = await _tagService.SearchTagAsync(null);

                var categoryList = categories.Select(c => new CategoryData {
                    Id = c.CategoryId,
                    Name = c.CategoryName
                }).ToList();

                var tagList = tags.Select(t => new TagData {
                    Id = t.TagId,
                    Name = t.TagName!
                }).ToList();

                return ViewComponent("ArticleCreateUpdateForm", new {
                    actionType = actionType,
                    createUpdateArticleParams = createUpdateArticleParams,
                    categorieList = categoryList,
                    tagList = tagList,
                });
            }

            try {
                switch (actionType) {
                    case ActionType.Create:
                    Console.WriteLine(JsonSerializer.Serialize(createUpdateArticleParams));
                    var createRequest = new CreateNewsArticleRequest {
                        NewsTitle = createUpdateArticleParams.Title,
                        Headline = createUpdateArticleParams.Headline,
                        NewsContent = createUpdateArticleParams.Content,
                        NewsSource = createUpdateArticleParams.Source,
                        CategoryId = createUpdateArticleParams.CategoryId ?? 0,
                        TagIds = createUpdateArticleParams.TagIds,
                        CreatedById = HttpContext.Session.GetInt32("AccountId") ?? 0,
                    };

                    await _newsArticleService.AddNewsArticleAsync(createRequest);
                    break;

                    case ActionType.Update:
                    var updateRequest = new UpdateNewsArticleRequest {
                        NewsArticleId = createUpdateArticleParams.Id,
                        NewsTitle = createUpdateArticleParams.Title,
                        Headline = createUpdateArticleParams.Headline,
                        NewsContent = createUpdateArticleParams.Content,
                        NewsSource = createUpdateArticleParams.Source,
                        CategoryId = createUpdateArticleParams.CategoryId ?? 0,
                        UpdatedById = HttpContext.Session.GetInt32("AccountId") ?? 0,
                        TagIds = createUpdateArticleParams.TagIds
                    };

                    await _newsArticleService.UpdateNewsArticleAsync(updateRequest);
                    break;
                }

                return new JsonResult(new { success = true });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("Title", ex.Message);
                Response.StatusCode = 400;

                var categories = await _categoryService.SearchCategoryAsync(null, null);
                var tags = await _tagService.SearchTagAsync(null);

                var categoryList = categories.Select(c => new CategoryData {
                    Id = c.CategoryId,
                    Name = c.CategoryName
                }).ToList();

                var tagList = tags.Select(t => new TagData {
                    Id = t.TagId,
                    Name = t.TagName!
                }).ToList();

                return ViewComponent("ArticleCreateUpdateForm", new {
                    actionType = actionType,
                    createUpdateArticleParams = createUpdateArticleParams,
                    categorieList = categoryList,
                    tagList = tagList,
                });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(string articleId, bool isActive) {
            try {
                await _newsArticleService.ChangeStatusAsync(articleId, isActive ? Status.Active : Status.Inactive);

                return new JsonResult(new { success = true });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

    }
}
