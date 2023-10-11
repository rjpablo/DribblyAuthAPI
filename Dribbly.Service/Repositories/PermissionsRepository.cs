using Dribbly.Authentication.Enums;
using Dribbly.Authentication.Models;
using Dribbly.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Dribbly.Service.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private IAuthContext _authContext;

        public PermissionsRepository(IAuthContext authContext)
        {
            _authContext = authContext;
        }

        public async Task<IEnumerable<PermissionModel>> GetUserPermissionsAsync(string userId)
        {
            return await GetUserPermissionsAsync(_authContext, userId);
        }

        public static async Task<IEnumerable<PermissionModel>> GetUserPermissionsAsync
            (IAuthContext authContext, string userId)
        {
            List<int> userPermissionIds = await authContext.UserPermissions
                .Where(p => p.UserId == userId)
                .Select(p => p.PermissionId)
                .ToListAsync();

            return EnumFunctions.GetAllPermissions()
                .Where(p => userPermissionIds.Contains(p.Value))
                // TODO: It would be nicer if we could do this by casting so that we won't have to 
                // update this contructor in case we modify the properties of either class.
                .Select(p => new PermissionModel(p.Subject, p.Action, p.Value));
        }
    }
}
