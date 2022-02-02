using Courier.Authentication;
using Courier.Data.Models;

namespace Courier.Repositories;

public interface ITokenRepository : IPersonalAccessTokenStore
{
    Task<List<UserPersonalToken>> GetUserTokens(string userId);
    Task<UserPersonalToken> CreateRandomUserToken(string userId, string description, DateTime? expiresAt);
    Task UpdateUserToken(UserPersonalToken token);
    Task RemoveUserToken(UserPersonalToken token);
    Task<UserPersonalToken?> FindById(string userId, string id);
}