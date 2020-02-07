using DribblyAuthAPI.Models.Courts;
using System.Collections.Generic;

namespace DribblyAuthAPI.Services
{
    public interface ISettingsService
    {
        IEnumerable<SettingModel> GetInitialSettings();
        string GetValue(string key);
    }
}