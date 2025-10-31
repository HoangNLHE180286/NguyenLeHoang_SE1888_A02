using BusinessLogic.DTOs.Requests;
using BusinessLogic.Services;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Presentation.Models.Params;

namespace Presentation.Pages.Account {
    public class LoginModel : PageModel {
        [BindProperty]
        public LoginParams LoginParams { get; set; }

        private readonly SystemAccountService _systemAccountService;

        public LoginModel(SystemAccountService systemAccountService) {
            _systemAccountService = systemAccountService;
        }

        public async Task<IActionResult> OnPost([FromForm] LoginParams loginParams) {
            if (!ModelState.IsValid) {
                return Page();
            }

            try {
                var account = await _systemAccountService.LoginAsync(new LoginRequest { AccountEmail = loginParams.Email, AccountPassword = loginParams.Password });
                HttpContext.Session.SetInt32("AccountId", account.AccountId);
                HttpContext.Session.SetString("Username", account.AccountName ?? string.Empty);
                HttpContext.Session.SetString("UserRole", ((Role)account.AccountRole.Value).ToString());

                return RedirectToPage("/Index");
            } catch (InvalidOperationException ex) {
                ModelState.AddModelError("LoginParams.Email", ex.Message);
                return Page();
            } catch (UnauthorizedAccessException ex) {
                ModelState.AddModelError("LoginParams.Password", ex.Message);
                return Page();
            } catch (Exception ex) {
                TempData["Error"] = ex.Message;
                return Page();
            }
        }
    }
}
