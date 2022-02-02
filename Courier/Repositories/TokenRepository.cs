using Courier.Data;
using Courier.Data.Models;
using Courier.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Courier.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly CourierDbContext _context;
    
    public TokenRepository(CourierDbContext context)
    {
        _context = context;
    }
    
    public Task<List<UserPersonalToken>> GetUserTokens(string userId)
    {
        return _context.UserPersonalTokens.AsNoTrackingWithIdentityResolution()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserPersonalToken> CreateRandomUserToken(string userId, string description, DateTime? expiresAt)
    {
        var token = new UserPersonalToken
        {
            Id = Guid.NewGuid().ToString(),
            Token = TokenHelper.GenerateRandomToken("pt"),
            Description = description,
            UserId = userId,
            ExpiresAt = expiresAt
        };

        await _context.UserPersonalTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        return token;
    }

    public async Task UpdateUserToken(UserPersonalToken token)
    {
        _context.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserToken(UserPersonalToken token)
    {
        _context.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task<UserPersonalToken?> FindById(string userId, string id)
    {
        return await _context.UserPersonalTokens.AsNoTrackingWithIdentityResolution()
            .Where(t => t.UserId == userId)
            .SingleOrDefaultAsync(t => t.Id == id);
    }

    public async Task<UserPersonalToken?> FindByToken(string token)
    {
        return await _context.UserPersonalTokens.AsNoTrackingWithIdentityResolution()
            .SingleOrDefaultAsync(t => t.Token == token);
    }
}