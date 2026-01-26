using Domain.Enums.AppRole;

namespace Application.Shared.Settings;

public static class RolePermissions
{
    public static readonly Dictionary<AppRoles, List<string>> Map =
        new()
        {
            [AppRoles.Admin] = new List<string>
            {
                //Permissions.Users.View,
                Permissions.Users.Create,
                Permissions.Users.Update,
                Permissions.Users.Delete,
                Permissions.Dashboard.View
            },
            [AppRoles.Moderator] = new List<string>
            {
                //Permissions.Users.View,
                Permissions.Users.Update,
                Permissions.Dashboard.View
            },
            [AppRoles.User] = new List<string>
            {
                Permissions.Dashboard.View
            }
        };
}
