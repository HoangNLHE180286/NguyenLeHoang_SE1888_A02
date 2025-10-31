using BusinessLogic.DTOs.Requests;
using BusinessLogic.Services;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;

namespace Presentation.Pages {  // Giữ nguyên namespace root cho home
    public class IndexModel : PageModel {
        private readonly NewsArticleService _newsArticleService;

        public IndexModel(NewsArticleService newsArticleService) {
            _newsArticleService = newsArticleService ?? throw new ArgumentNullException(nameof(newsArticleService));  // CHANGE: Null check để tránh service null
        }

        // ... (giữ nguyên các method khác nếu có, ví dụ OnGetAsync) ...

        // CHANGE: Handler với try-catch để debug error, và filter Status = Active cho home (chỉ show published)
        public async Task<IActionResult> OnGetHomeArticleListAsync() {
            try {
                var request = new SearchNewsArticleRequest() {
                    Keyword = null,
                    Category = null,
                    Status = Status.Active,  // CHANGE: Chỉ load Active articles cho home (an toàn, tránh show draft)
                    FromDate = null,
                    ToDate = null,
                    Author = null
                };

                var articles = await _newsArticleService.SearchNewsArticleAsync(request);  // CHANGE: Gọi service, nếu null/empty thì OK

                var list = articles?.Select(x => new ArticleData() {
                    Id = x.NewsArticleId,
                    Title = x.NewsTitle ?? "-",  // CHANGE: Null-safe để tránh crash
                    Headline = x.Headline ?? "",
                    Content = x.NewsContent ?? "",
                    Category = x.Category?.CategoryName ?? "-",
                    Source = x.NewsSource ?? "-",
                    Status = x.NewsStatus ?? Status.Inactive,  // CHANGE: Default nếu null
                    Author = x.CreatedBy?.AccountName ?? "Unknown",
                    CreatedAt = x.CreatedDate ?? DateTime.Now,  // CHANGE: Default date
                    Tags = x.Tags?.Select(y => new TagData() { Id = y.TagId, Name = y.TagName ?? "" }).ToList() ?? new List<TagData>()
                }).ToList() ?? new List<ArticleData>();  // CHANGE: Null-safe mapping

                // CHANGE: Return ViewComponent, nếu list empty thì vẫn render "No articles"
                return ViewComponent("HomeArticleList", list);
            } catch (Exception ex) {
                // CHANGE: Catch và return error HTML để debug (xem trong Network response)
                var errorHtml = $@"
                    <div class='alert alert-danger text-center'>
                        <strong>Lỗi load articles:</strong> {ex.Message}<br/>
                        <small>Stack: {ex.StackTrace?.Substring(0, 200)}...</small>
                    </div>";
                return Content(errorHtml);  // Trả HTML error thay vì crash AJAX
            }
        }
    }
}