using Dribbly.Model.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task<AccountModel> GetAccountByIdentityId(long userId);

        Task<AccountModel> GetAccountById(long Id);

        Task<AccountBasicInfoModel> GetAccountBasicInfo(long userId);

        Task<long?> GetIdentityUserAccountId(string identityUserId);

        IQueryable<AccountModel> SearchAccounts(AccountSearchInputModel input);
    }
}