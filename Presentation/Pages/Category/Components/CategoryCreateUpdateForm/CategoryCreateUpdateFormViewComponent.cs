using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;
using Presentation.Models.Params;

namespace Presentation.Pages.Category.Components.CreateUpdateForm {
    public class CategoryCreateUpdateFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(ActionType actionType, CreateUpdateCategoryParams createUpdateCategoryParams, List<CategoryData> categories) {
            ViewData["ActionType"] = actionType;
            ViewData["Categories"] = categories;
            return View(createUpdateCategoryParams);
        }
    }
}
