using Courier.Data.Models;
using Courier.Repositories;
using Courier.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.SinglePackage;

public class Home : PageModel
{
    public string PackageName { get; set; } = null!;
    public Package Package { get; set; } = null!;

    private readonly IPackageRepository _packageRepository;
    private readonly IPermissionChecker _permissionChecker;
    private readonly UserManager<AppUser> _userManager;

    private string UserId => _userManager.GetUserId(User);

    public Home(IPackageRepository packageRepository, UserManager<AppUser> userManager, IPermissionChecker permissionChecker)
    {
        _packageRepository = packageRepository;
        _userManager = userManager;
        _permissionChecker = permissionChecker;
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

        Package = package;
        PackageName = packageName;
            
        return Page();
    }
}