using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Courier.Repositories;
using Courier.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.SinglePackage;

public class Members : PageModel
{
    public string PackageName { get; set; } = null!;
    public Package Package { get; set; } = null!;
    public List<PackageUser> Users { get; set; } = null!;
    public string? AddedPackageUserId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;
        
    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Has write access")]
        public bool CanWrite { get; set; } = false;
    }

    private readonly IPackageRepository _packageRepository;
    private readonly IPermissionChecker _permissionChecker;
    private readonly UserManager<AppUser> _userManager;

    private string? _userId;
    public string UserId => _userId ??= _userManager.GetUserId(User);
        
    public Members(IPackageRepository packageRepository, UserManager<AppUser> userManager, IPermissionChecker permissionChecker)
    {
        _packageRepository = packageRepository;
        _userManager = userManager;
        _permissionChecker = permissionChecker;
    }
        
    public async Task<ActionResult> OnGetAsync(string packageName, string? addedPackageUserId = null)
    {
        var package = await _packageRepository.FindPackage(packageName);
        if (package == null)
        {
            return NotFound();
        }

        if (!await _permissionChecker.CanUserReadPackage(UserId, package.Id))
        {
            return Forbid();
        }
            
        PackageName = packageName;
        Package = package;
        Users = await _packageRepository.GetPackageUsers(package);
        AddedPackageUserId = addedPackageUserId;

        return Page();
    }

    public async Task<ActionResult> OnPostAddPackageUserAsync(string packageName, string packageId)
    {
        var user = await _userManager.FindByNameAsync(Input.UserName);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "User is not found");
            return Page();
        }

        var package = await _packageRepository.FindPackage(packageName);
        if (package is null || package.Id != packageId)
        {
            return BadRequest();
        }

        if (package.OwnerId != UserId)
        {
            return Forbid();
        }

        var packageUser = await _packageRepository.AddUserToPackage(package.Id, user.Id, Input.CanWrite);
        var addedPackageUserId = packageUser?.Id;

        return RedirectToPage(new{packageName, addedPackageUserId});
    }

    public async Task<ActionResult> OnPostUpdatePackageUserCanWriteAsync(string packageName, string packageId,
        string packageUserId, bool packageUserCanWrite)
    {
        var package = await _packageRepository.FindPackage(packageName);
        if (package is null || package.Id != packageId)
        {
            return BadRequest();
        }

        if (package.OwnerId != UserId)
        {
            return Forbid();
        }

        await _packageRepository.UpdatePackageUserWritePermission(packageId, packageUserId, packageUserCanWrite);

        return RedirectToPage(new{packageName});
    }

    public async Task<ActionResult> OnPostRemovePackageUserAsync(string packageName, string packageId,
        string packageUserId)
    {
        var package = await _packageRepository.FindPackage(packageName);
        if (package is null || package.Id != packageId)
        {
            return BadRequest();
        }
            
        var packageUser = await _packageRepository.FindPackageUserById(packageId, packageUserId);
        if (packageUser is null)
        {
            return BadRequest();
        }
            
        if (package.OwnerId != UserId && UserId != packageUser.UserId)
        {
            return Forbid();
        }

        await _packageRepository.RemovePackageUser(package.Id, packageUser.Id);
            
        return RedirectToPage(new{packageName}); 
    }
}