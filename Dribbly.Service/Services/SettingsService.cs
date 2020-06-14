using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dribbly.Service.Models;
using Dribbly.Service.Models.Courts;

namespace Dribbly.Service.Services
{
    public class SettingsService : BaseService<SettingModel>, ISettingsService
    {
        IAuthContext _context;
        public SettingsService(IAuthContext context):base(context.Settings)
        {
            _context = context;
        }

        public IEnumerable<SettingModel> GetInitialSettings()
        {
            return All();
        }

        public string GetValue(string key)
        {
            SettingModel setting = SingleOrDefault(s => s.Key == key);
            if(setting != null)
            {
                return setting.Value ?? setting.DefaultValue;
            }
            else
            {
                throw new Exception("A setting with the key \"" + key + "\" does not exist.");
            }
        }
    }
}