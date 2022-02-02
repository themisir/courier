using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages;

public class Logout : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public Logout(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<ActionResult> OnGetAsync()
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect("~/");
    }
}