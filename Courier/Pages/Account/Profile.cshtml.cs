using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.Account;

public class Profile : PageModel
{
    public AppUser CurrentUser { get; set; } = null!;

    [BindProperty] public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress]
        [StringLength(50, MinimumLength = 3)]
        public string Email { get; set; } = null!;

        [Display(Name = "Display name")]
        [StringLength(50)]
        public string? FullName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;
    }

    private readonly UserManager<AppUser> _userManager;

    public Profile(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User);
        Input = new InputModel
        {
            Email = CurrentUser.Email,
            UserName = CurrentUser.UserName,
            FullName = CurrentUser.FullName
        };
    }

    public async Task<ActionResult> OnPostAsync()
    {
        CurrentUser = await _userManager.GetUserAsync(User);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.FullName != CurrentUser.FullName)
        {
            CurrentUser.FullName = string.IsNullOrWhiteSpace(Input.FullName) ? null : Input.FullName;
            await _userManager.UpdateAsync(CurrentUser);
        }
            
        if (Input.Email != CurrentUser.Email)
        {
            var result = await _userManager.SetEmailAsync(CurrentUser, Input.Email);
            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("Email", identityError.Description);
                }
            }
        }

        if (Input.UserName != CurrentUser.UserName)
        {
            var result = await _userManager.SetUserNameAsync(CurrentUser, Input.UserName);
            if (!result.Succeeded)
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("Email", identityError.Description);
                }
            }
        }

        return Page();
    }
}