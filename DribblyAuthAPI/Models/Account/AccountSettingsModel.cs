namespace DribblyAuthAPI.Models.Account
{
    public class AccountSettingsModel
    {
        // Profile Settings
        public bool KeepProfilePrivate { get; set; }
        public bool ShowBirthDate { get; set; } = true;
    }
}