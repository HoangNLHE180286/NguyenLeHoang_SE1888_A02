using BusinessLogic.DTOs.Requests;
using BusinessLogic.DTOs.Response;
using BusinessLogic.Rules;
using BusinessLogic.Validation;
using Core.Enums;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class NewsArticleService {
        private readonly INewsArticleRepository _newsArticleRepository;
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly ITagRepository _tagRepository;
        private readonly NewsArticleValidator _newsArticleValidator;
        private readonly NewsArticleRules _newsArticleRules;

        public NewsArticleService(INewsArticleRepository newsArticleRepository, ISystemAccountRepository systemAccountRepository, ITagRepository tagRepository, NewsArticleValidator newsArticleValidator, NewsArticleRules newsArticleRules) {
            _newsArticleRepository = newsArticleRepository;
            _systemAccountRepository = systemAccountRepository;
            _tagRepository = tagRepository;
            _newsArticleValidator = newsArticleValidator;
            _newsArticleRules = newsArticleRules;
        }
        public async Task<IEnumerable<NewsArticleResponse>> SearchNewsArticleAsync(SearchNewsArticleRequest searchNewsArticleRequest) {
            bool? isActive = searchNewsArticleRequest.Status == null
                ? null
                : searchNewsArticleRequest.Status == Status.Active;

            // 🔹 1️⃣ Lấy danh sách bài viết thỏa điều kiện
            var newsArticles = await _newsArticleRepository.SearchNewsArticleAsync(
                searchNewsArticleRequest.Keyword,
                searchNewsArticleRequest.Author,
                searchNewsArticleRequest.Category,
                isActive,
                searchNewsArticleRequest.FromDate,
                searchNewsArticleRequest.ToDate
            );

            // 🔹 2️⃣ Danh sách kết quả
            var result = new List<NewsArticleResponse>();

            // 🔹 3️⃣ Duyệt từng bài viết & xử lý lần lượt
            foreach (var newsArticle in newsArticles) {
                // ⚠️ Gọi tuần tự, mỗi lần await một truy vấn — tránh lỗi DbContext
                var createdBy = await _systemAccountRepository.GetAccountByIdAsync((short)newsArticle.CreatedById);
                dynamic updatedBy = null!;

                if (newsArticle.UpdatedById != null) {
                    updatedBy = await _systemAccountRepository.GetAccountByIdAsync((short)newsArticle.UpdatedById);
                }

                // 🔹 4️⃣ Map sang DTO (Response)
                result.Add(new NewsArticleResponse {
                    NewsArticleId = newsArticle.NewsArticleId,
                    NewsTitle = newsArticle.NewsTitle,
                    Headline = newsArticle.Headline,
                    CreatedDate = newsArticle.CreatedDate,
                    NewsContent = newsArticle.NewsContent,
                    NewsSource = newsArticle.NewsSource == "N/A" ? "Unknown" : newsArticle.NewsSource,
                    Category = newsArticle.Category == null ? null : new CategoryResponse {
                        CategoryId = newsArticle.Category.CategoryId,
                        CategoryName = newsArticle.Category.CategoryName,
                        CategoryDescription = newsArticle.Category.CategoryDesciption,
                        Status = newsArticle.Category.IsActive == true ? Status.Active : Status.Inactive,
                    },
                    NewsStatus = newsArticle.NewsStatus == true ? Status.Active : Status.Inactive,
                    CreatedBy = createdBy == null ? null : new SystemAccountResponse {
                        AccountId = createdBy.AccountId,
                        AccountName = createdBy.AccountName,
                    },
                    UpdatedBy = updatedBy == null ? null : new SystemAccountResponse {
                        AccountId = updatedBy.AccountId,
                        AccountName = updatedBy.AccountName,
                    },
                    ModifiedDate = newsArticle.ModifiedDate,
                    Tags = newsArticle.Tags == null ? null : newsArticle.Tags.Select(t => new TagResponse {
                        TagId = t.TagId,
                        TagName = t.TagName,
                    }).ToList(),
                });
            }

            return result;
        }

        public async Task<NewsArticleResponse> GetNewsArticleByIdAsync(string newsArticleId) {
            var newsArticle = await _newsArticleRepository.GetNewsArticleByIdAsync(newsArticleId);
            var createdBy = await _systemAccountRepository.GetAccountByIdAsync((short)newsArticle.CreatedById);

            dynamic updatedBy = null!;

            if (newsArticle.UpdatedById != null) {
                updatedBy = await _systemAccountRepository.GetAccountByIdAsync((short)newsArticle.UpdatedById);
            }

            return new NewsArticleResponse {
                NewsArticleId = newsArticleId,
                NewsTitle = newsArticle.NewsTitle,
                Headline = newsArticle.Headline,
                CreatedDate = newsArticle.CreatedDate,
                NewsContent = newsArticle.NewsContent,
                NewsSource = newsArticle.NewsSource == "N/A" ? "Unknown" : newsArticle.NewsSource,
                Category = newsArticle.Category == null ? null : new CategoryResponse {
                    CategoryId = newsArticle.Category.CategoryId,
                    CategoryName = newsArticle.Category.CategoryName,
                    CategoryDescription = newsArticle.Category.CategoryDesciption,
                    Status = newsArticle.Category.IsActive == true ? Status.Active : Status.Inactive,
                },
                NewsStatus = newsArticle.NewsStatus == true ? Status.Active : Status.Inactive,
                CreatedBy = createdBy == null ? null : new SystemAccountResponse {
                    AccountId = createdBy.AccountId,
                    AccountName = newsArticle.CreatedBy.AccountName,
                },
                UpdatedBy = updatedBy == null ? null : new SystemAccountResponse {
                    AccountId = updatedBy.AccountId,
                    AccountName = updatedBy.AccountName,
                },
                ModifiedDate = newsArticle.ModifiedDate,
                Tags = newsArticle.Tags == null ? null : newsArticle.Tags.Select(t => new TagResponse {
                    TagId = t.TagId,
                    TagName = t.TagName,
                }).ToList(),
            };
        }


        public async Task AddNewsArticleAsync(CreateNewsArticleRequest createNewsArticleRequest) {
            _newsArticleValidator.ValidateForCreate(createNewsArticleRequest);
            await _newsArticleRules.CheckForCreate(createNewsArticleRequest);
            var tags = await _tagRepository.GetListTagByIdsAsync(createNewsArticleRequest.TagIds);

            await _newsArticleRepository.AddNewsArticleAsync(new NewsArticle {
                NewsTitle = createNewsArticleRequest.NewsTitle,
                Headline = createNewsArticleRequest.Headline,
                NewsContent = createNewsArticleRequest.NewsContent,
                CategoryId = (createNewsArticleRequest.CategoryId == null || createNewsArticleRequest.CategoryId == 0) ? null : (short)createNewsArticleRequest.CategoryId,
                NewsSource = createNewsArticleRequest.NewsSource,
                NewsStatus = true,
                CreatedById = (short)createNewsArticleRequest.CreatedById,
                Tags = tags.ToList()
            });
        }

        public async Task UpdateNewsArticleAsync(UpdateNewsArticleRequest updateNewsArticleRequest) {
            _newsArticleValidator.ValidateForUpdate(updateNewsArticleRequest);
            await _newsArticleRules.CheckForUpdate(updateNewsArticleRequest);

            var tags = await _tagRepository.GetListTagByIdsAsync(updateNewsArticleRequest.TagIds);

            await _newsArticleRepository.UpdateNewsArticleAsync(new NewsArticle {
                NewsArticleId = updateNewsArticleRequest.NewsArticleId,
                NewsTitle = updateNewsArticleRequest.NewsTitle,
                Headline = updateNewsArticleRequest.Headline,
                NewsContent = updateNewsArticleRequest.NewsContent,
                CategoryId = (updateNewsArticleRequest.CategoryId == null || updateNewsArticleRequest.CategoryId == 0) ? null : (short)updateNewsArticleRequest.CategoryId,
                NewsSource = updateNewsArticleRequest.NewsSource,
                UpdatedById = (short)updateNewsArticleRequest.UpdatedById,
                Tags = (ICollection<Tag>)tags
            });
        }

        public async Task DeleteNewsArticleAsync(string newsArticleId) {

            await _newsArticleRepository.DeleteNewsArticleAsync(newsArticleId);
        }

        public async Task DuplicateArticleAsync(string articleId) {
            var original = await _newsArticleRepository.GetNewsArticleByIdAsync(articleId);
            if (original == null)
                throw new InvalidOperationException("Original article not found.");

            // Lấy danh sách bài có cùng tiêu đề gốc (bao gồm các bản copy)
            var existingArticles = await _newsArticleRepository.SearchNewsArticleAsync(original.NewsTitle, null, null, null, null, null);
            int copyCount = existingArticles
                .Count(a => a.NewsTitle.StartsWith(original.NewsTitle));

            // Tạo tiêu đề mới không trùng
            string newTitle = copyCount == 0
                ? original.NewsTitle + " (Copy)"
                : $"{original.NewsTitle} (Copy {copyCount})";

            var tagIds = original.Tags?.Select(t => t.TagId).ToList();
            var tags = await _tagRepository.GetListTagByIdsAsync(tagIds);

            var duplicate = new NewsArticle {
                NewsTitle = newTitle,
                Headline = original.Headline,
                NewsContent = original.NewsContent,
                NewsSource = original.NewsSource,
                CategoryId = original.CategoryId,
                NewsStatus = original.NewsStatus,
                CreatedById = original.CreatedById,
                CreatedDate = DateTime.Now,
                ModifiedDate = null,
                UpdatedById = null,
                Tags = tags.ToList()
            };

            await _newsArticleRepository.AddNewsArticleAsync(duplicate);
        }


        public async Task ChangeStatusAsync(string articleId, Status status) {
            var article = await _newsArticleRepository.GetNewsArticleByIdAsync(articleId);

            article.NewsStatus = status == Status.Active;

            await _newsArticleRepository.UpdateNewsArticleAsync(article);
        }


    }
}
