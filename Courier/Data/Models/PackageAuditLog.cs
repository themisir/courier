namespace Courier.Data.Models;

public class PackageAuditLog
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string PackageId { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string? EventDescription { get; set; }
    public Dictionary<string, object> Meta { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser? User { get; set; }
    public Package? Package { get; set; }
}