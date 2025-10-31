using BusinessLogic.DTOs;
using BusinessLogic.DTOs.Requests;
using BusinessLogic.DTOs.Response;
using BusinessLogic.Rules;
using BusinessLogic.Validation;
using Core.Enums;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services {
    public class SystemAccountService {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly SystemAccountValidator _systemAccountValidator;
        private readonly SystemAccountRules _systemAccountRules;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository, IConfiguration configuration, SystemAccountValidator systemAccountValidator, SystemAccountRules systemAccountRules) {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _systemAccountValidator = systemAccountValidator;
            _systemAccountRules = systemAccountRules;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest) {
            _systemAccountValidator.ValidateForLogin(loginRequest);
            await _systemAccountRules.CheckForLogin(loginRequest);

            var email = loginRequest.AccountEmail;
            var password = loginRequest.AccountPassword;

            var adminEmail = _configuration["AdminAccount:Email"];
            if (email == adminEmail) {

                var adminName = _configuration["AdminAccount:Name"];
                var adminRole = int.Parse(_configuration["AdminAccount:Role"]);
                var adminPassword = _configuration["AdminAccount:Password"]; 


                var existingAdmin = await _systemAccountRepository.GetAccountAsync(adminEmail);
                if (existingAdmin == null) {
                    var adminAccount = new SystemAccount {
                        AccountId = 0,
                        AccountEmail = adminEmail,
                        AccountName = adminName,
                        AccountPassword = adminPassword,
                        AccountRole = (int)Role.Admin,
                    };
                    await _systemAccountRepository.AddAccountAsync(adminAccount);

                    var admin = await _systemAccountRepository.GetAccountAsync(adminEmail);
                    return new LoginResponse {
                        AccountId = admin.AccountId,
                        AccountEmail = adminEmail,
                        AccountName = adminName,
                        AccountRole = adminRole
                    };
                }
                return new LoginResponse {
                    AccountId = existingAdmin.AccountId,
                    AccountEmail = adminEmail,
                    AccountName = adminName,
                    AccountRole = adminRole
                };
            }

            var account = await _systemAccountRepository.GetAccountAsync(email);
            return new LoginResponse {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };
        }

        public async Task<IEnumerable<SystemAccountResponse>> SearchAccountAsync(string? keyword, int? accountRole) {
            var accounts = await _systemAccountRepository.SearchAccountAsync(keyword, accountRole);
            return accounts.Select(a => new SystemAccountResponse {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountEmail = a.AccountEmail,
                AccountRole = a.AccountRole
            }).ToList();
        }

        public async Task AddAccountAsync(CreateAccountRequest createAccountRequest) {
            _systemAccountValidator.ValidateForCreate(createAccountRequest);
            await _systemAccountRules.CheckForCreateAccount(createAccountRequest);

            var account = new SystemAccount {
                AccountName = createAccountRequest.AccountName,
                AccountEmail = createAccountRequest.AccountEmail,
                AccountPassword = createAccountRequest.AccountPassword,
                AccountRole = createAccountRequest.AccountRole
            };

            await _systemAccountRepository.AddAccountAsync(account);
        }

        public async Task<SystemAccountResponse> GetAccountByIdAsync(int accountId) {
            var account = await _systemAccountRepository.GetAccountByIdAsync(accountId);

            return new SystemAccountResponse {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole
            };
        }

        public async Task DeleteAccountAsync(int accountId) {
            await _systemAccountRules.CheckForDeleteAccount(accountId);
            await _systemAccountRepository.DeleteAccountAsync(accountId);
        }

        public async Task UpdateAccountAsync(UpdateAccountRequest updateAccountRequest) {
            await _systemAccountRules.CheckForUpdateAccount(updateAccountRequest);
            var account = new SystemAccount {
                AccountId = (short)updateAccountRequest.AccountId,
                AccountName = updateAccountRequest.AccountName,
                AccountEmail = updateAccountRequest.AccountEmail,
                AccountRole = updateAccountRequest.AccountRole
            };
            await _systemAccountRepository.UpdateAccountAsync(account);
        }

        public async Task UpdatePasswordAsync(UpdatePasswordRequest updatePasswordRequest) {

            _systemAccountValidator.ValidateForUpdatePassword(updatePasswordRequest);
            await _systemAccountRules.CheckForUpdatePassword(updatePasswordRequest);
            await _systemAccountRepository.UpdateAccountAsync(new SystemAccount { AccountId = (short)updatePasswordRequest.AccountId, AccountPassword = updatePasswordRequest.NewPassword });
        }
    }
}
