using Courier.Data.Models;
using Courier.Models;
using Courier.Models.Dto;
using Courier.Repositories;
using Courier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Courier.Controllers;

[Route("api/packages")]
[Authorize("Bearer")]
[ApiController]
public class PackagesController : ControllerBase
{
    private readonly IPackageRepository _packageRepository;
    private readonly IPermissionChecker _permissionChecker;
    private readonly UserManager<AppUser> _userManager;
    private readonly IOptions<ServerDetails> _serverDetails;

    public PackagesController(IPackageRepository packageRepository, IPermissionChecker permissionChecker,
        UserManager<AppUser> userManager, IOptions<ServerDetails> serverDetails)
    {
        _packageRepository = packageRepository;
        _permissionChecker = permissionChecker;
        _userManager = userManager;
        _serverDetails = serverDetails;
    }

    private string UserId => _userManager.GetUserId(User);

    [HttpGet("{packageName}")]
    public async Task<ActionResult<PackageDto>> GetPackage(string packageName)
    {
        var package = await _packageRepository.FindPackageDto(packageName);
        if (package == null)
        {
            return NotFound(MessageResponse.Error("Package is not found"));
        }

        if (!await _permissionChecker.CanUserReadPackage(UserId, package.Id))
        {
            return Forbid();
        }

        return Ok(package);
    }

    [HttpGet("versions/new")]
    public ActionResult<PendingUploadDto> CreateNewPackageVersion()
    {
        return Ok(new PendingUploadDto
        {
            Url = Url.Action("UploadArchive", "Archives", null, _serverDetails.Value.BaseUrl?.Scheme ?? Request.Scheme)
        });
    }

    [HttpGet("{packageName}/versions/{versionName}")]
    public async Task<ActionResult<PackageVersionDto>> GetPackageVersion(string packageName, string versionName)
    {
        var package = await _packageRepository.FindPackageDto(packageName);
        if (package == null)
        {
            return NotFound(MessageResponse.Error("Package is not found"));
        }

        if (!await _permissionChecker.CanUserReadPackage(UserId, package.Id))
        {
            return Forbid();
        }

        var version = package.Versions.FirstOrDefault(v => v.Version == versionName);
        if (version == null)
        {
            return NotFound(MessageResponse.Error("Version is not found"));
        }

        return Ok(version);
    }
}