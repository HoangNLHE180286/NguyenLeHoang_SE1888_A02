using BusinessLogic.DTOs.Requests;
using Core.Enums;
using DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Rules {
    public class SystemAccountRules {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly INewsArticleRepository _newsArticleRepository;

        public SystemAccountRules(ISystemAccountRepository systemAccountRepository, INewsArticleRepository newsArticleRepository, IConfiguration configuration) {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task CheckForLogin(LoginRequest loginRequest) {
            await this.CheckSystemAccountAsync(loginRequest.AccountEmail, CheckMode.MustExist);

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (loginRequest.AccountEmail == adminEmail) {
                if (loginRequest.AccountPassword != adminPassword) {
                    throw new UnauthorizedAccessException("Password is incorrect!");
                } else {
                    return;
                }
            }

            var systemAccount = await _systemAccountRepository.GetAccountAsync(loginRequest.AccountEmail);
            if (systemAccount.AccountPassword != loginRequest.AccountPassword) {
                throw new UnauthorizedAccessException("Password is incorrect!");
            }
        }

        public async Task CheckForCreateAccount(CreateAccountRequest createAccountRequest) {
            await this.CheckSystemAccountAsync(createAccountRequest.AccountEmail, CheckMode.MustNotExist);
        }

        public async Task CheckForUpdateAccount(UpdateAccountRequest updateAccountRequest) {
            var currentAccount = await _systemAccountRepository.GetAccountByIdAsync(updateAccountRequest.AccountId);
            if (currentAccount.AccountEmail != updateAccountRequest.AccountEmail) {
                await this.CheckSystemAccountAsync(updateAccountRequest.AccountEmail, CheckMode.MustNotExist);
            }
        }

        public async Task CheckForUpdatePassword(UpdatePasswordRequest updatePasswordRequest) {
            var account = await _systemAccountRepository.GetAccountByIdAsync(updatePasswordRequest.AccountId);
            if (account.AccountPassword != updatePasswordRequest.OldPassword) {
                throw new UnauthorizedAccessException("Password is incorrect!");
            }
        }

        public async Task CheckForDeleteAccount(int accountId) {
            var articleQuantity = await _newsArticleRepository.CountNewsArticlesByAccountIdAsync(accountId);
            if (articleQuantity > 0)
                throw new InvalidOperationException("This account has already created some articles. Cannot delete it!");
        }

        // General rules
        private async Task CheckSystemAccountAsync(string email, CheckMode mode) {
            var adminEmail = _configuration["AdminAccount:Email"];

            var systemAccount = await _systemAccountRepository.GetAccountAsync(email);
            switch (mode) {
                case CheckMode.MustExist:
                if (email == adminEmail)
                    return;
                if (systemAccount == null)
                    throw new InvalidOperationException("Account doesn't exist!");
                break;

                case CheckMode.MustNotExist:
                if (email == adminEmail || systemAccount != null)
                    throw new InvalidOperationException("Account has already existed!");
                break;
            }
        }


    }
}
