namespace Dribbly.Authentication.Models
{
    public class PermissionModel
    {
        public int Value { get; set; }
        public string Subject { get; set; }
        public string Action { get; set; }
        public string Name
        {
            get => Subject + '.' + Action;
        }
        public string DescriptionKey
        {
            get { return "Permission_" + Subject + Action; }
        }

        public PermissionModel(string subject, string action, int value)
        {
            Subject = subject;
            Action = action;
            Value = value;
        }
    }
}
