using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace Courier.Helpers;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    private readonly SnakeCaseNamingStrategy _newtonsoftSnakeCaseNamingStrategy = new();

    public static SnakeCaseNamingPolicy Instance { get; } = new();

    public override string ConvertName(string name)
    {
        return _newtonsoftSnakeCaseNamingStrategy.GetPropertyName(name, false);
    }
}