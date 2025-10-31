using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Params;

namespace Presentation.Pages.Account.Components.CreateUpdateForm {
    public class AccountCreateUpdateFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(ActionType actionType, CreateUpdateAccountParams? createUpdateAccountParams) {
            ViewData["ActionType"] = actionType;
            return View(createUpdateAccountParams);
        }
    }
}
