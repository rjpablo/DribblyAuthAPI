using DribblyAuthAPI.Models;
using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;
using System.Data.Entity;

namespace DribblyAuthAPI.Repositories
{
    public class SettingsRepository : BaseRepository<SettingModel>
    {
        AuthContext _context;

        public SettingsRepository(IAuthContext context) :base(context.Settings)
        {
            _context = new AuthContext();
        }

        public IEnumerable<SettingModel> GetAll()
        {
            return _context.Settings;
        }

    }
}