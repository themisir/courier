using Microsoft.EntityFrameworkCore;

namespace Courier.Data.Models;

[Index(nameof(Token), IsUnique = true)]
public class UserPersonalToken
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public AppUser? User { get; set; }
}