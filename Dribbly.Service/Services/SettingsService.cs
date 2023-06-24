using Dribbly.Model;
using Dribbly.Model.Courts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dribbly.Service.Services
{
    public class SettingsService : BaseService<SettingModel>, ISettingsService
    {
        IAuthContext _context;
        public SettingsService(IAuthContext context) : base(context.Settings)
        {
            _context = context;
        }

        public IEnumerable<SettingModel> GetInitialSettings()
        {
            var settings = All().ToList();

            settings.Add(new SettingModel()
            {
                Key = "Fouls",
                Value = JsonConvert.SerializeObject(Constants.Fouls, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
                    Formatting = Formatting.Indented
                })
            });
            return settings;
        }

        public string GetValue(string key)
        {
            SettingModel setting = SingleOrDefault(s => s.Key == key);
            if (setting != null)
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