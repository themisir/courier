namespace Courier.Storage.Local;

public class LocalArchiveManagerOptions
{
    public string UriPrefix { get; set; } = default!;
    public string Origin { get; set; } = default!;
    public string Directory { get; set; } = default!;
}