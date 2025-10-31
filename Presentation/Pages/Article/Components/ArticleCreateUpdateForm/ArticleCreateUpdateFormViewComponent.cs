using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presentation.Models.Data;
using Presentation.Models.Params;

namespace Presentation.Pages.Article.Components.ArticleCreateUpdateForm {
    public class ArticleCreateUpdateFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(ActionType actionType, List<CategoryData> categorieList, List<TagData> tagList, CreateUpdateArticleParams? createUpdateArticleParams) {
            ViewData["ActionType"] = actionType;
            ViewData["Categories"] = categorieList.Select(c => new SelectListItem() { Text = c.Name, Value = c.Id.ToString(), Selected = actionType == ActionType.Update && c.Id == createUpdateArticleParams?.CategoryId }).ToList();
            ViewData["Tags"] = tagList.Select(t => new SelectListItem() { Text = t.Name, Value = t.Id.ToString(), Selected = createUpdateArticleParams?.TagIds?.Contains(t.Id) ?? false }).ToList();
            return View(createUpdateArticleParams);
        }
    }
}
