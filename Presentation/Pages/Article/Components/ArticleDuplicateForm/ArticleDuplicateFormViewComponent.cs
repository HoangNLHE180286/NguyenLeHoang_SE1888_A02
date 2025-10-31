using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Article.Components.ArticleDuplicateForm {
    public class ArticleDuplicateFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(string name) {
            return View("Default", name);
        }
    }
}
