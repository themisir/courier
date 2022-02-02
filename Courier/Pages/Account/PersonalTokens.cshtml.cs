using System.ComponentModel.DataAnnotations;
using Courier.Data.Models;
using Courier.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Courier.Pages.Account;

public class PersonalTokens : PageModel
{
    public List<UserPersonalToken> Tokens { get; set; } = null!;
    public UserPersonalToken? IssuedToken { get; set; }
            
    [BindProperty] public InputModel Input { get; set; } = null!;
        
    public class InputModel
    {
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = null!;
            
        [Display(Name = "Expires at")]
        public DateTime? ExpiresAt { get; set; }
    }
        
    private readonly ITokenRepository _tokenRepository;
    private readonly UserManager<AppUser> _userManager;

    private string? _userId;
    private string UserId => _userId ??= _userManager.GetUserId(User);

    public PersonalTokens(ITokenRepository tokenRepository, UserManager<AppUser> userManager)
    {
        _tokenRepository = tokenRepository;
        _userManager = userManager;
    }
        
    public async Task OnGetAsync(string? issuedTokenId = null)
    {
        Tokens = await _tokenRepository.GetUserTokens(UserId);
        if ((IssuedToken = Tokens.FirstOrDefault(t => t.Id == issuedTokenId)) is not null)
        {
            if (IssuedToken.CreatedAt > DateTime.UtcNow.AddMinutes(-5))
            {
                Tokens.Remove(IssuedToken);
            }
            else
            {
                IssuedToken = null;
            }
        }
    }

    public async Task<ActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
            
        IssuedToken = await _tokenRepository.CreateRandomUserToken(UserId, Input.Description, Input.ExpiresAt);

        return RedirectToPage("/Account/PersonalTokens", new { issuedTokenId = IssuedToken?.Id });
    }

    public async Task<ActionResult> OnPostRemoveAsync(string tokenId)
    {
        var token = await _tokenRepository.FindById(UserId, tokenId);
        if (token == null)
        {
            return NotFound();
        }

        await _tokenRepository.RemoveUserToken(token);
        return RedirectToPage("/Account/PersonalTokens");
    }
}