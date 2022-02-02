namespace Courier.Models.Dto;

public class PendingUploadDto
{
    public string? Url { get; set; } = string.Empty;
    public Dictionary<string, string> Fields { get; set; } = new();
}