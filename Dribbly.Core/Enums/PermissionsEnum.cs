using static Dribbly.Core.Extensions.EnumExtensions;

/// <summary>
/// Each class under this namespace must be added to EnumFunctions.GetAllPermissions.
/// UserPermission seeder may also need to be updated.
/// </summary>
namespace Dribbly.Core.Enums.Permissions
{
    public enum CourtPermission
    {
        [EnumAttribute(Subject = "Court", Action = "UpdateNotOwned")]
        UpdateNotOwned = 1,
        [EnumAttribute(Subject = "Court", Action = "DeletePhotoNotOwned")]
        DeletePhotoNotOwned = 2,
        [EnumAttribute(Subject = "Court", Action = "AddVideoNotOwned")]
        AddVideoNotOwned = 3,
        [EnumAttribute(Subject = "Court", Action = "DeleteVideoNotOwned")]
        DeleteVideoNotOwned = 4,
    }

    public enum GamePermission
    {
        [EnumAttribute(Subject = "Game", Action = "Approve")]
        Approve = 11,
    }

    public enum AccountPermission
    {
        [EnumAttribute(Subject = "Account", Action = "UpdatePhotoNotOwned")]
        UpdatePhotoNotOwned = 21,
        [EnumAttribute(Subject = "Account", Action = "DeletePhotoNotOwned")]
        DeletePhotoNotOwned = 22,
        [EnumAttribute(Subject = "Account", Action = "AddVideoNotOwned")]
        AddVideoNotOwned = 23,
        [EnumAttribute(Subject = "Account", Action = "UpdateNotOwned")]
        UpdateNotOwned = 24
    }
}
