using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;

namespace Presentation.Pages.Category.Components.CategoryList {
    public class CategoryListViewComponent : ViewComponent {
        public IViewComponentResult Invoke(List<CategoryData> categories) {
            return View(categories);
        }
    }
}
