using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Params;

namespace Presentation.Pages.Account.Components.UpdatePasswordForm {
    public class UpdatePasswordFormViewComponent : ViewComponent {
        public IViewComponentResult Invoke(string name) {
            UpdatePasswordParams updatePasswordParams = new UpdatePasswordParams {
                Name = name
            };
            return View(updatePasswordParams);
        }
    }
}
