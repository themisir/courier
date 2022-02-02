using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages;

public class Index : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public Index(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public ActionResult OnGet()
    {
        return LocalRedirect(_signInManager.IsSignedIn(User) ? "~/Account/Dashboard" : "~/Login");
    }
}