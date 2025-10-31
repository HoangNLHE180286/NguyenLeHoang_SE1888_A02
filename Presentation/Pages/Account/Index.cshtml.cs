using BusinessLogic.DTOs.Requests;
using BusinessLogic.Services;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Data;
using Presentation.Models.Params;

namespace Presentation.Pages.Account {
    public class IndexModel : PageModel {

        private readonly SystemAccountService _systemAccountService;
        public IndexModel(SystemAccountService systemAccountService) {
            _systemAccountService = systemAccountService;
        }

        [BindProperty(SupportsGet = true)]
        public SearchAccountParams SearchParams { get; set; } = new();

        public List<AccountData> Accounts { get; set; } = new List<AccountData>();

        public async Task<IActionResult> OnGetSearchAccountAsync() {
            var keyword = string.IsNullOrWhiteSpace(SearchParams.Keyword) ? null : SearchParams.Keyword;
            int? role = null;

            if (!string.IsNullOrWhiteSpace(SearchParams.Role)) {
                role = (int)(Role)Enum.Parse(typeof(Role), SearchParams.Role);
            }

            var accounts = await _systemAccountService.SearchAccountAsync(keyword, role);

            Accounts = accounts.Select(a => new AccountData {
                Id = a.AccountId,
                Name = a.AccountName,
                Email = a.AccountEmail,
                Role = ((Role)a.AccountRole.Value).ToString()
            }).ToList();

            return ViewComponent("AccountList", Accounts);
        }

        public async Task<IActionResult> OnGetOpenModalAsync(ActionType actionType, int? accountId) {
            switch (actionType) {
                case ActionType.Create:
                return ViewComponent("AccountCreateUpdateForm", new { actionType = ActionType.Create });
                case ActionType.Update:
                var account = await _systemAccountService.GetAccountByIdAsync(accountId.Value);
                return ViewComponent("AccountCreateUpdateForm", new {
                    actionType = ActionType.Update, createUpdateAccountParams = new CreateUpdateAccountParams {
                        Name = account.AccountName,
                        Email = account.AccountEmail,
                        Role = ((Role)account.AccountRole.Value).ToString()
                    }
                });
                case ActionType.Delete:
                account = await _systemAccountService.GetAccountByIdAsync(accountId.Value);
                return ViewComponent("AccountDeleteForm", account.AccountName);
                case ActionType.UpdatePassword:
                account = await _systemAccountService.GetAccountByIdAsync(accountId.Value);
                return ViewComponent("UpdatePasswordForm", account.AccountName);
                default:
                return null;
            }
        }

        public async Task<IActionResult> OnPostCreateUpdateAsync(ActionType actionType, [FromForm] CreateUpdateAccountParams createUpdateAccountParams) {
            if (!ModelState.IsValid) {
                return ViewComponent("AccountCreateUpdateForm", new { actionType = actionType, createUpdateAccountParams = createUpdateAccountParams });
            }

            try {
                switch (actionType) {
                    case ActionType.Create:
                    var createParams = new CreateAccountRequest {
                        AccountName = createUpdateAccountParams.Name,
                        AccountEmail = createUpdateAccountParams.Email,
                        AccountPassword = createUpdateAccountParams.Password,
                        AccountRole = (int)Enum.Parse(typeof(Role), createUpdateAccountParams.Role)
                    };

                    await _systemAccountService.AddAccountAsync(createParams);
                    break;
                    case ActionType.Update:
                    var updateParams = new UpdateAccountRequest {
                        AccountId = createUpdateAccountParams.Id,
                        AccountName = createUpdateAccountParams.Name,
                        AccountEmail = createUpdateAccountParams.Email,
                        AccountRole = (int)Enum.Parse(typeof(Role), createUpdateAccountParams.Role)
                    };

                    await _systemAccountService.UpdateAccountAsync(updateParams);
                    break;
                }
                return new JsonResult(new { success = true });
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("Email", ex.Message);
                Response.StatusCode = 400;
                return ViewComponent("AccountCreateUpdateForm", new { actionType = actionType, createUpdateAccountParams = createUpdateAccountParams });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostUpdatePasswordAsync([FromForm] UpdatePasswordParams updatePasswordParams) {
            var account = await _systemAccountService.GetAccountByIdAsync(updatePasswordParams.Id);
            if (!ModelState.IsValid) {
                Response.StatusCode = 400;
                return ViewComponent("UpdatePasswordForm", account.AccountName);
            }

            try {
                var updatePassParams = new UpdatePasswordRequest {
                    AccountId = updatePasswordParams.Id,
                    OldPassword = updatePasswordParams.OldPassword,
                    NewPassword = updatePasswordParams.NewPassword,
                };

                await _systemAccountService.UpdatePasswordAsync(updatePassParams);

                return new JsonResult(new { success = true });
            } catch (UnauthorizedAccessException ex) {
                ModelState.AddModelError("OldPassword", ex.Message);
                Response.StatusCode = 400;
                return ViewComponent("UpdatePasswordForm", account.AccountName);
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int accountId) {
            try {
                await _systemAccountService.DeleteAccountAsync(accountId);
                return new JsonResult(new { success = true });
            } catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        
    }
}
