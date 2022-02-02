using System.Security.Cryptography;
using System.Text;

namespace Courier.Helpers;

public class GravatarHelper
{
    public static string GenerateAvatar(string identifier)
    {
        using var md5 = MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(identifier.ToLowerInvariant()));
        var hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        return $"https://www.gravatar.com/avatar/{hash}?s=80&d=mp&r=g";
    }
}