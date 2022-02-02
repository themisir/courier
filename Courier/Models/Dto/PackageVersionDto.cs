using System.Text.Json.Serialization;

namespace Courier.Models.Dto;

public class PackageVersionDto
{
    public string Version { get; set; } = null!;
    public Dictionary<string, object> Pubspec { get; set; } = new();
    public string ArchiveUrl { get; set; } = string.Empty;
    [JsonIgnore] public string ArchiveKey { get; set; } = string.Empty;
    public DateTime Published { get; set; }
}