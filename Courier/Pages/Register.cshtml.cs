using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Courier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Courier.Pages;

public class Register : PageModel
{
    public Register(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IOptions<AuthenticationOptions> authenticationSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authenticationSettings = authenticationSettings;
    }

    [BindProperty] public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;
            
        [Required]
        [Display(Name = "Email address")]
        [EmailAddress]
        [StringLength(50, MinimumLength = 3)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }

    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IOptions<AuthenticationOptions> _authenticationSettings;
        
    public void OnGet(string? email = null)
    {
        if (email is not null)
        {
            Input.Email = email;
        }
    }

    public async Task<ActionResult> OnPost(string? returnUrl = null)
    {
        if (!_authenticationSettings.Value.AllowSignUp)
        {
            return Forbid();
        }
            
        if (!ModelState.IsValid)
        {
            return Page();
        }
            
        returnUrl ??= Url.Content("~/");
            
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new AppUser
        {
            UserName = Input.UserName,
            Email = Input.Email,
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: "Password");
            return LocalRedirect(returnUrl);
        }

        foreach (var identityError in result.Errors)
        {
            ModelState.AddModelError(string.Empty, identityError.Description);
        }

        return Page();
    }
}