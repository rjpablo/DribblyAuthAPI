using Dribbly.Model.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<PlayerModel> GetAccountByUsername(string userName);

        Task<PlayerModel> GetAccountByIdentityId(long userId);

        Task<PlayerModel> GetAccountById(long Id);

        Task<AccountBasicInfoModel> GetAccountBasicInfo(long userId);

        Task<long?> GetIdentityUserAccountId(long identityUserId);

        Task<long> GetIdentityUserId(long accountId);

        Task<long> GetIdentityUserAccountIdNotNullAsync(long identityUserId);

        IQueryable<PlayerModel> SearchAccounts(AccountSearchInputModel input);
    }
}