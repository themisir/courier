namespace Courier.Models.Dto;

public class PackageDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public PackageVersionDto? Latest { get; set; }
    public List<PackageVersionDto> Versions { get; set; } = new();
}