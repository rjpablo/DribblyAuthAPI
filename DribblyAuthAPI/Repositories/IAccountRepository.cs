using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Account;
using DribblyAuthAPI.Models.Auth;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DribblyAuthAPI.Repositories
{
    public interface IAccountRepository: IDisposable
    {
        Task<AccountModel> GetAccountByUsername(string userName);
    }
}