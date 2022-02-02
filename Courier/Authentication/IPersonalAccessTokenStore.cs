using Courier.Data.Models;

namespace Courier.Authentication;

public interface IPersonalAccessTokenStore
{
    Task<UserPersonalToken?> FindByToken(string token);
}