using Microsoft.AspNetCore.Mvc;
using Presentation.Models.Data;

namespace Presentation.Pages.Account.Components.AccountList {
    public class AccountListViewComponent : ViewComponent {
        public IViewComponentResult Invoke(List<AccountData> accounts) {
            return View(accounts);
        }
    }
}
