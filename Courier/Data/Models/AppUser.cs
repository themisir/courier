using Microsoft.AspNetCore.Identity;

namespace Courier.Data.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<UserPersonalToken>? PersonalTokens { get; set; }
    public List<Package>? Packages { get; set; }
    
    public string PrimaryName => string.IsNullOrEmpty(FullName) ? $"@{UserName}" : FullName;
    public string? SecondaryName => string.IsNullOrEmpty(FullName) ? null : $"@{UserName}";
}