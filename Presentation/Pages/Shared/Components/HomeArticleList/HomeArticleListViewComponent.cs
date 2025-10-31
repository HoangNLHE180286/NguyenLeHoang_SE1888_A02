using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Data;

namespace Presentation.Pages.Home.Components.HomeArticleList {  // CHANGE: Namespace OK, nhưng đảm bảo khớp folder
    public class HomeArticleListViewComponent : ViewComponent {
        public IViewComponentResult Invoke(List<ArticleData> articles) {
            // CHANGE: Null-check input
            if (articles == null) articles = new List<ArticleData>();
            return View(articles);
        }
    }
}