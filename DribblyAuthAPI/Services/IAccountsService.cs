using DribblyAuthAPI.Models.Account;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Services
{
    public interface IAccountsService
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task AddAsync(AccountModel account);
    }
}