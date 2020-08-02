using Dribbly.Model.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task<AccountModel> GetAccountById(string userId);

        Task<AccountBasicInfoModel> GetAccountBasicInfo(string userId);

        IQueryable<AccountModel> SearchAccounts(AccountSearchInputModel input);
    }
}