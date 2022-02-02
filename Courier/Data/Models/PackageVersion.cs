using Courier.Data.Helpers;

namespace Courier.Data.Models;

public class PackageVersion
{
    public string Id { get; set; } = IdHelper.GenerateId();
    public string PackageId { get; set; } = null!;
    public string VersionName { get; set; } = null!;
    public string? ArchiveKey { get; set; }
    public string? ReadmeContents { get; set; }
    public string? ChangelogContents { get; set; }
    public string UserId { get; set; } = null!;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Package? Package { get; set; }
    public AppUser? User { get; set; }
}