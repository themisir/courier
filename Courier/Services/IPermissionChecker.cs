namespace Courier.Services;

public interface IPermissionChecker
{
    Task<bool> CanUserReadPackage(string userId, string packageId);
    Task<bool> CanUserWritePackage(string userId, string packageId);
}