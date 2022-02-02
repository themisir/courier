using Courier.Data.Models;
using Courier.Repositories;
using Courier.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.SinglePackage;

public class Settings : PageModel
{
    public string PackageName { get; set; } = null!;
    public Package Package { get; set; } = null!;

    private readonly IPackageRepository _packageRepository;
    private readonly IPermissionChecker _permissionChecker;
    private readonly UserManager<AppUser> _userManager;
        
    private string? _userId;
    public string UserId => _userId ??= _userManager.GetUserId(User);

    public Settings(IPackageRepository packageRepository, IPermissionChecker permissionChecker, UserManager<AppUser> userManager)
    {
        _packageRepository = packageRepository;
        _permissionChecker = permissionChecker;
        _userManager = userManager;
    }
        
    public async Task<ActionResult> OnGetAsync(string packageName)
    {
        var package = await _packageRepository.FindPackage(packageName);
        if (package is null)
        {
            return NotFound();
        }

        if (!await _permissionChecker.CanUserReadPackage(UserId, package.Id))
        {
            return Forbid();
        }

        PackageName = packageName;
        Package = package;

        return Page();
    }
}