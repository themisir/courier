using Courier.Data.Models;
using Courier.Helpers;
using Courier.Models;
using Courier.Repositories;
using Courier.Services;
using Courier.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Courier.Controllers;

[Route("api/packages/archives")]
[Authorize("Bearer")]
[ApiController]
public class ArchivesController : ControllerBase
{
    private readonly IArchiveManager _archiveManager;
    private readonly IPackageRepository _packageRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ArchivesController> _logger;
    private readonly IPermissionChecker _permissionChecker;
    private readonly IOptions<ServerDetails> _serverDetails;

    private string UserId => _userManager.GetUserId(User);

    public ArchivesController(IArchiveManager archiveManager, IPackageRepository packageRepository,
        UserManager<AppUser> userManager, ILogger<ArchivesController> logger, IPermissionChecker permissionChecker,
        IOptions<ServerDetails> serverDetails)
    {
        _archiveManager = archiveManager;
        _packageRepository = packageRepository;
        _userManager = userManager;
        _logger = logger;
        _permissionChecker = permissionChecker;
        _serverDetails = serverDetails;
    }

    [HttpGet("done")]
    public ActionResult<SuccessMessageResponse> Done()
    {
        return Ok(MessageResponse.Success("Uploaded!"));
    }
        
    [HttpPost("upload")]
    public async Task<ActionResult> UploadArchive(IFormFile file)
    {
        await using var fileStream = file.OpenReadStream();
        await using var memoryStream = new MemoryStream();
            
        _logger.LogInformation("Reading file contents from request");
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation("Reading archive contents");
        var archiveContents = await ArchiveHelper.ReadPackageArchive(memoryStream);
        if (archiveContents.Pubspec is null)
        {
            return UploadFailed("Uploaded archive does not contains a valid pubspec.yaml file");
        }
            
        if (archiveContents.Pubspec["name"] is not string packageName)
        {
            return UploadFailed("Pubspec.yaml does not contains package name");
        }
            
        if (archiveContents.Pubspec["version"] is not string versionName)
        {
            return UploadFailed("Pubspec.yaml does not contains version value");
        }

        if (!Version.TryParse(versionName, out var versionValue))
        {
            return UploadFailed("Pubspec.yaml contains invalid version value");
        }
            
        var package = await _packageRepository.FindPackage(packageName = packageName.ToLowerInvariant());
        if (package is null)
        {
            _logger.LogInformation("Package with name {PackageName} is not exists. Creating a new one",
                packageName);
            var result = await _packageRepository.CreatePackage(packageName, UserId);
            if (result.Succeeded)
            {
                package = result.Package!;
            }
            else
            {
                return UploadFailed(string.Join("; ", result.Errors));
            }
        }
        else
        {
            if (!await _permissionChecker.CanUserWritePackage(UserId, package.Id))
            {
                return UploadFailed("Uploader does not have required permissions to upload a new version");
            }
                
            var latestVersion = package.LatestVersion;
            if (latestVersion is not null)
            {
                if (versionValue < new Version(latestVersion.VersionName))
                {
                    return UploadFailed($"Uploaded version should be higher than latest version: {versionName} < {latestVersion.VersionName}");
                }
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
            
        var packageVersion = new PackageVersion
        {
            Package = package,
            PackageId = package.Id,
            UserId = UserId,
            VersionName = versionName,
            Metadata = archiveContents.Pubspec,
            ReadmeContents = archiveContents.Readme,
            ChangelogContents = archiveContents.Changelog,
        };
            
        _logger.LogInformation("Uploading archive to the archive storage");
        var archiveStorageKey = await _archiveManager.Upload(memoryStream, packageVersion);
        if (archiveStorageKey == null)
        {
            return UploadFailed("Failed to upload archive to storage");
        }

        packageVersion.Package = null;
        packageVersion.ArchiveKey = archiveStorageKey;

        _logger.LogInformation("Creating a new package version for package {PackageName} {VersionName}",
            package.Name, packageVersion.VersionName);
        await _packageRepository.CreatePackageVersion(packageVersion);

        return RedirectToAction(nameof(Done), "Archives", null,
            _serverDetails.Value.BaseUrl?.Scheme ?? Request.Scheme);
    }

    private ActionResult UploadFailed(string message)
    {
        _logger.LogError("Archive upload failed. {Message}", message);
        return BadRequest(MessageResponse.Error(message));
    }
}