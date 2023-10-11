namespace Dribbly.Model.Account
{
    public class AccountSettingsModel
    {
        // Profile Settings
        public bool IsPublic { get; set; }
        public bool ShowBirthDate { get; set; } = true;
    }
}