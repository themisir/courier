using Courier.Data;
using Microsoft.EntityFrameworkCore;

namespace Courier.Services;

public class PermissionChecker : IPermissionChecker
{
    private readonly CourierDbContext _context;

    public PermissionChecker(CourierDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanUserReadPackage(string userId, string packageId)
    {
        return await _context.Packages.AnyAsync(p => p.OwnerId == userId && p.Id == packageId) ||
               await _context.PackageUsers.AnyAsync(p => p.UserId == userId && p.PackageId == packageId);
    }

    public async Task<bool> CanUserWritePackage(string userId, string packageId)
    {
        return await _context.Packages.AnyAsync(p => p.OwnerId == userId && p.Id == packageId) ||
               await _context.PackageUsers.AnyAsync(p => p.UserId == userId &&
                                                         p.PackageId == packageId &&
                                                         p.CanUpload);
    }
}