using Courier.Data.Models;
using Courier.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.Account;

public class Dashboard : PageModel
{
    public List<Package> Packages { get; set; } = null!;

    public string? SearchQuery
    {
        set => ViewData["SearchQuery"] = value;
        get => ViewData["SearchQuery"]?.ToString();
    }

    private readonly UserManager<AppUser> _userManager;
    private readonly IPackageRepository _packageRepository;
        
    private string? _userId;
    private string UserId => _userId ??= _userManager.GetUserId(User);

    public Dashboard(UserManager<AppUser> userManager, IPackageRepository packageRepository)
    {
        _userManager = userManager;
        _packageRepository = packageRepository;
    }

    public async Task OnGetAsync(string? query = null)
    {
        SearchQuery = query;

        if (string.IsNullOrWhiteSpace(query))
        {
            Packages = await _packageRepository.GetUserPackages(UserId);
        }
        else
        {
            Packages = await _packageRepository.SearchUserPackages(UserId, query);
        }
    }
}