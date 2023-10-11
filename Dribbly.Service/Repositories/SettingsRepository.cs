using Dribbly.Model;
using Dribbly.Model.Courts;
using System.Collections.Generic;

namespace Dribbly.Service.Repositories
{
    public class SettingsRepository
    {
        AuthContext _context;

        public SettingsRepository(IAuthContext context)
        {
            _context = new AuthContext();
        }

        public IEnumerable<SettingModel> GetAll()
        {
            return _context.Settings;
        }

    }
}