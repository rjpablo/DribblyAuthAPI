using Dribbly.Model.Courts;
using System.Collections.Generic;

namespace Dribbly.Service.Services
{
    public interface ISettingsService
    {
        IEnumerable<SettingModel> GetInitialSettings();
        string GetValue(string key);
    }
}