using Courier.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Courier.Data.Models;

[Index(nameof(Name), IsUnique = false)]
public class Package
{
    public string Id { get; set; } = IdHelper.GenerateId();
    public string Name { get; set; } = string.Empty;
    public string OwnerId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public AppUser? Owner { get; set; }
    public List<PackageVersion>? Versions { get; set; }
    public List<PackageUser>? Users { get; set; }
    public List<PackageAuditLog>? AuditLogs { get; set; }
    
    public PackageVersion? LatestVersion => Versions?.FirstOrDefault();
}
