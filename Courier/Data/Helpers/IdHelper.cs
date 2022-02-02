namespace Courier.Data.Helpers;

public static class IdHelper
{
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }
}