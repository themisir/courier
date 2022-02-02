using System.Linq;
using AutoMapper;
using Courier.Data;
using Courier.Data.Models;
using Courier.Models;
using Courier.Models.Dto;
using Courier.Storage;
using Microsoft.EntityFrameworkCore;

namespace Courier.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly CourierDbContext _context;
    private readonly IArchiveManager _archiveManager;
    private readonly IMapper _mapper;

    public PackageRepository(CourierDbContext context, IArchiveManager archiveManager, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _archiveManager = archiveManager;
    }

    private IQueryable<Package> Packages => _context.Packages
        .AsNoTrackingWithIdentityResolution()
        .Include(p => p.Versions)
        .Include(p => p.Owner);

    private IQueryable<Package> UserPackages(string userId) => Packages
        .Where(p => p.OwnerId == userId || p.Users!.Any(u => u.UserId == userId));

    private async Task EnrichVersion(PackageVersionDto version)
    {
        version.ArchiveUrl = await _archiveManager.GetDownloadUrl(version.ArchiveKey);
    }

    private async Task EnrichPackage(PackageDto package)
    {
        if (package.Latest is not null) await EnrichVersion(package.Latest);
        await Task.WhenAll(package.Versions.Select(EnrichVersion));
    }

    public async Task<PackageDto?> FindPackageDto(string name)
    {
        var package = await FindPackage(name);
        var result = _mapper.Map<PackageDto>(package);
        if (result is not null) await EnrichPackage(result);
        return result;
    }

    public async Task<PublishPackageResult> CreatePackage(string name, string userId)
    {
        name = name.ToLowerInvariant();

        if (!name.All(c => char.IsDigit(c) || c is >= 'a' and <= 'z' or '_'))
        {
            return PublishPackageResult.Error("Name contains invalid characters");
        }

        if (await _context.Packages.AnyAsync(p => p.Name == name))
        {
            return PublishPackageResult.Error("Package name is already in use");
        }

        var package = new Package
        {
            Name = name,
            OwnerId = userId,
        };

        await _context.Packages.AddAsync(package);
        await _context.SaveChangesAsync();

        return PublishPackageResult.Success(package);
    }

    public async Task<Package?> FindPackage(string name)
    {
        var package = await Packages.SingleOrDefaultAsync(p => p.Name == name);
        if (package is not null)
        {
            package.Versions = package.Versions?.OrderByDescending(v => v.CreatedAt).ToList();
        }

        return package;
    }

    public async Task<List<Package>> GetUserPackages(string userId)
    {
        var packages = await UserPackages(userId).ToListAsync();
        return packages;
    }

    public async Task<List<Package>> SearchUserPackages(string userId, string query)
    {
        query = query.ToLowerInvariant();
        
        var packages = await UserPackages(userId)
            .Where(q => EF.Functions.Like(q.Name, $"%{query}%"))
            .ToListAsync();
        return packages;
    }

    public async Task<List<PackageUser>> GetPackageUsers(Package package)
    {
        var users = await _context.PackageUsers
            .AsNoTrackingWithIdentityResolution()
            .Include(p => p.User)
            .ToListAsync();
        return users;
    }

    public async Task<PackageUser?> FindPackageUserById(string packageId, string packageUserId)
    {
        return await _context.PackageUsers.SingleOrDefaultAsync(pu => pu.Id == packageUserId && 
                                                                      pu.PackageId == packageId);
    }

    public async Task<PackageUser?> AddUserToPackage(string packageId, string userId, bool canUpload)
    {
        if (await _context.PackageUsers.AnyAsync(pu => pu.UserId == userId &&
                                                       pu.PackageId == packageId))
        {
            return null;
        }

        var packageUser = new PackageUser
        {
            PackageId = packageId,
            UserId = userId,
            CanUpload = canUpload,
        };

        await _context.PackageUsers.AddAsync(packageUser);
        await _context.SaveChangesAsync();

        return packageUser;
    }

    public async Task UpdatePackageUserWritePermission(string packageId, string packageUserId, bool canUpload)
    {
        var packageUser = await _context.PackageUsers
            .Where(pu => pu.PackageId == packageId)
            .SingleOrDefaultAsync(pu => pu.Id == packageUserId);

        if (packageUser is not null)
        {
            packageUser.CanUpload = canUpload;

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemovePackageUser(string packageId, string packageUserId)
    {
        var packageUser = await _context.PackageUsers
            .Where(pu => pu.PackageId == packageId)
            .SingleOrDefaultAsync(pu => pu.Id == packageUserId);

        if (packageUser is not null)
        {
            _context.PackageUsers.Remove(packageUser);
                
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreatePackageVersion(PackageVersion packageVersion)
    {
        await _context.PackageVersions.AddAsync(packageVersion);
        await _context.SaveChangesAsync();
    }
}