using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.Account;

public class ChangePassword : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
        
    public ChangePassword(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
        
    public void OnGet()
    {
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
            
        var user = await _userManager.GetUserAsync(User);
        var result =await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var identityError in result.Errors)
            {
                ModelState.AddModelError(string.Empty, identityError.Description);
            }

            return Page();
        }
            
        await _signInManager.RefreshSignInAsync(user);
            
        return RedirectToPage();
    }
}