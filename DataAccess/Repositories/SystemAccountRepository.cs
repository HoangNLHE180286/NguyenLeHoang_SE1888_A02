using DataAccess.Context;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public class SystemAccountRepository : ISystemAccountRepository {
        private readonly ApplicationDbContext _dbContext;

        public SystemAccountRepository(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task AddAccountAsync(SystemAccount account) {
            var lastId = await this.GetLastIdAsync();
            account.AccountId = ++lastId;
            await _dbContext.SystemAccounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(int accountId) {
            await _dbContext.SystemAccounts.Where(c => c.AccountId == accountId).ExecuteDeleteAsync();
        }

        public async Task<SystemAccount> GetAccountAsync(string accountEmail) {
            return await _dbContext.SystemAccounts.FirstOrDefaultAsync(c => c.AccountEmail == accountEmail);
        }

        public async Task<SystemAccount> GetAccountByIdAsync(int accountId) {
            return await _dbContext.SystemAccounts.FirstOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task<IEnumerable<SystemAccount>> SearchAccountAsync(string? keyword, int? accountRole) {
            keyword = keyword?.ToLower();
            var query = _dbContext.SystemAccounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword)) {
                query = query.Where(a => a.AccountName.ToLower().Contains(keyword) || a.AccountEmail.ToLower().Contains(keyword));
            }

            if (accountRole.HasValue) {
                query = query.Where(a => a.AccountRole == accountRole.Value);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateAccountAsync(SystemAccount updatedAccount) {
            var existing = await _dbContext.SystemAccounts.FindAsync(updatedAccount.AccountId);

            if (!string.IsNullOrEmpty(updatedAccount.AccountName)) {
                existing.AccountName = updatedAccount.AccountName;
            }

            if (!string.IsNullOrEmpty(updatedAccount.AccountEmail)) {
                existing.AccountEmail = updatedAccount.AccountEmail;
            }

            if (!string.IsNullOrEmpty(updatedAccount.AccountPassword)) {
                existing.AccountPassword = updatedAccount.AccountPassword;
            }

            if (updatedAccount.AccountRole.HasValue) {
                existing.AccountRole = updatedAccount.AccountRole.Value;
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<short> GetLastIdAsync() {
            return await _dbContext.SystemAccounts
                .OrderByDescending(a => a.AccountId)
                .Select(a => a.AccountId)
                .FirstOrDefaultAsync();
        }
    }
}
