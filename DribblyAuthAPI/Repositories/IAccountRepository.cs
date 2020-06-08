using DribblyAuthAPI.Models.Account;
using System;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<AccountModel> GetAccountByUsername(string userName);

        Task<AccountBasicInfoModel> GetAccountBasicInfo(string userId);
    }
}