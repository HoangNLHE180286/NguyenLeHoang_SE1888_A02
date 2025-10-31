using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presentation.Pages.Category.Components.CategoryDeleteForm {
    public class CategoryDeleteFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(string name) {
            return View("Default", name);
        }
    }
}
