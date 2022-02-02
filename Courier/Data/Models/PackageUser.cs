using Courier.Data.Helpers;

namespace Courier.Data.Models;

public class PackageUser
{
    public string Id { get; set; } = IdHelper.GenerateId();
    public string PackageId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Permissions
    public bool CanUpload { get; set; } = false;

    public Package? Package { get; set; }
    public AppUser? User { get; set; }
}