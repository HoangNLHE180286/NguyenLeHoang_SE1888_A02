using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories {
    public interface ISystemAccountRepository {
        Task<SystemAccount> GetAccountAsync(string accountEmail);
        Task<SystemAccount> GetAccountByIdAsync(int id);
        Task<IEnumerable<SystemAccount>> SearchAccountAsync(string? keyword, int? accountRole);
        Task AddAccountAsync(SystemAccount account);
        Task UpdateAccountAsync(SystemAccount account);
        Task DeleteAccountAsync(int accountId);
    }
}
