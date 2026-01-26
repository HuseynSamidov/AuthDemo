namespace Application.Shared.Settings;

public static class Permissions
{
    public static class Dashboard
    {
        public const string View = "Permissions.Dashboard.View";
    }
    public static class Users
    {
        //public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Update = "Permissions.Users.Update";
        public const string Delete = "Permissions.Users.Delete";
    }
}
