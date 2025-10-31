using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;

namespace Presentation.Pages.Article.Components.ArticleList {
    public class ArticleListViewComponent : ViewComponent {
        public IViewComponentResult Invoke(List<ArticleData> articles) {
            return View(articles);
        }
    }
}
