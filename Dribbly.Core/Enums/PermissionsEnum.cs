using static Dribbly.Core.Extensions.EnumExtensions;

namespace Dribbly.Core.Enums.Permissions
{
    public enum CourtPermission
    {
        [EnumAttribute(Subject = "Court", Action = "UpdateNotOwned")]
        UpdateNotOwned = 1
    }

    public enum GamePermission
    {
        [EnumAttribute(Subject = "Game", Action = "Approve")]
        Approve = 11,
    }
}
