using System.Security.Cryptography;
using System.Text;

namespace Courier.Helpers;

public class TokenHelper
{
    internal const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

    public static string GenerateRandomToken(string purpose, int size = 32)
    {
        return $"{purpose}_{GetUniqueKey(size)}";
    }
        
    public static string GetUniqueKey(int size)
    {            
        var data = RandomNumberGenerator.GetBytes(size * 4);
        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % Chars.Length;

            result.Append(Chars[(int)idx]);
        }

        return result.ToString();
    }
}