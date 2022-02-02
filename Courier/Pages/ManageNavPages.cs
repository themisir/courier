using Microsoft.AspNetCore.Mvc.Rendering;

namespace Courier.Pages
{
    public static class ManageNavPages
    {
        public const string Dashboard = "Dashboard";
        public const string PersonalTokens = "PersonalTokens";
        public const string Profile = "Profile";
        public const string ChangePassword = "ChangePassword";
        public const string TwoFactorAuthentication = "TwoFactorAuthentication";
        public const string Home = "Home";
        public const string Changelog = "Changelog";
        public const string Members = "Members";
        public const string Settings = "Settings";

        public static string? DashboardNavClass(ViewContext viewContext) => PageNavClass(viewContext, Dashboard);

        public static string? PersonalTokensNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalTokens);
        
        public static string? ProfileNavClass(ViewContext viewContext) => PageNavClass(viewContext, Profile);

        public static string? ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string? TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

        public static string? HomeNavClass(ViewContext viewContext) => PageNavClass(viewContext, Home);
        
        public static string? MembersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Members);
        
        public static string? ChangelogNavClass(ViewContext viewContext) => PageNavClass(viewContext, Changelog);

        public static string? SettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Settings);

        public static string? PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}