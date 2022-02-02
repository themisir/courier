using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages;

public class Login : PageModel
{
    public Login(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ILogger<Login> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
    }
        
    [BindProperty] public InputModel Input { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
        
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger _logger;

    public async Task<ActionResult> OnPost(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
            
        returnUrl ??= Url.Content("~/");
            
        var user = await _userManager.FindByNameAsync(Input.UserName);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return Page();
        }
            
        var result = await _signInManager.PasswordSignInAsync(user, Input.Password, Input.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in");
            return LocalRedirect(returnUrl);
        }
        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
        }
        if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out");
            return RedirectToPage("./Lockout");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}