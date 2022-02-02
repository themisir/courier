using Courier.Data.Models;
using Courier.Models;
using Courier.Models.Dto;

namespace Courier.Repositories;

public interface IPackageRepository
{
    Task<Package?> FindPackage(string name);
    Task<PackageDto?> FindPackageDto(string name);
    Task<PublishPackageResult> CreatePackage(string name, string userId);
    Task<List<Package>> GetUserPackages(string userId);
    Task<List<Package>> SearchUserPackages(string userId, string query);
    Task<List<PackageUser>> GetPackageUsers(Package package);

    Task<PackageUser?> FindPackageUserById(string packageId, string packageUserId);
    Task<PackageUser?> AddUserToPackage(string packageId, string userId, bool canUpload);
    Task UpdatePackageUserWritePermission(string packageId, string packageUserId, bool canUpload);
    Task RemovePackageUser(string packageId, string packageUserId);

    Task CreatePackageVersion(PackageVersion packageVersion);
}