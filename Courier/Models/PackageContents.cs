namespace Courier.Models;

public class PackageContents
{
    public Dictionary<string, object>? Pubspec { get; set; }
    public string? Readme { get; set; }
    public string? Changelog { get; set; }
}