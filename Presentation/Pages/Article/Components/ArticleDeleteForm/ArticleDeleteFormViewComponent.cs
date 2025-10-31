using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Article.Components.ArticleDeleteForm {
    public class ArticleDeleteFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(string name) {
            return View("Default", name);
        }
    }
}
